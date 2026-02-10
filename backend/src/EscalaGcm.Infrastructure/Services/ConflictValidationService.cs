using EscalaGcm.Application.DTOs.Escalas;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class ConflictValidationService : IConflictValidationService
{
    private readonly AppDbContext _context;
    public ConflictValidationService(AppDbContext context) => _context = context;

    public async Task<List<ConflictError>> ValidateAllocationsAsync(DateOnly data, int horarioId, List<AlocacaoRequest> alocacoes, int? excludeItemId = null)
    {
        var errors = new List<ConflictError>();

        foreach (var aloc in alocacoes)
        {
            if (aloc.GuardaId.HasValue)
            {
                var guardaId = aloc.GuardaId.Value;
                var guarda = await _context.Guardas.FindAsync(guardaId);
                var guardaNome = guarda?.Nome ?? $"Guarda #{guardaId}";

                // Check vacation
                var emFerias = await _context.Ferias.AnyAsync(f =>
                    f.GuardaId == guardaId && f.DataInicio <= data && f.DataFim >= data);
                if (emFerias)
                    errors.Add(new ConflictError("FERIAS", $"{guardaNome} está em férias na data {data:dd/MM/yyyy}"));

                // Check absence
                var emAusencia = await _context.Ausencias.AnyAsync(a =>
                    a.GuardaId == guardaId && a.DataInicio <= data && a.DataFim >= data);
                if (emAusencia)
                    errors.Add(new ConflictError("AUSENCIA", $"{guardaNome} possui ausência registrada na data {data:dd/MM/yyyy}"));

                // Check schedule conflict (same guard, same date, overlapping time)
                var conflito = await _context.EscalaAlocacoes
                    .Include(ea => ea.EscalaItem)
                    .AnyAsync(ea =>
                        ea.GuardaId == guardaId
                        && ea.EscalaItem.Data == data
                        && ea.EscalaItem.HorarioId == horarioId
                        && (excludeItemId == null || ea.EscalaItemId != excludeItemId));
                if (conflito)
                    errors.Add(new ConflictError("CONFLITO_ESCALA", $"{guardaNome} já está escalado na data {data:dd/MM/yyyy} no mesmo horário"));
            }

            if (aloc.EquipeId.HasValue)
            {
                var equipeId = aloc.EquipeId.Value;
                var membros = await _context.EquipeMembros
                    .Include(m => m.Guarda)
                    .Where(m => m.EquipeId == equipeId)
                    .ToListAsync();

                foreach (var membro in membros)
                {
                    var guardaNome = membro.Guarda.Nome;

                    var emFerias = await _context.Ferias.AnyAsync(f =>
                        f.GuardaId == membro.GuardaId && f.DataInicio <= data && f.DataFim >= data);
                    if (emFerias)
                        errors.Add(new ConflictError("FERIAS", $"{guardaNome} (equipe) está em férias na data {data:dd/MM/yyyy}"));

                    var emAusencia = await _context.Ausencias.AnyAsync(a =>
                        a.GuardaId == membro.GuardaId && a.DataInicio <= data && a.DataFim >= data);
                    if (emAusencia)
                        errors.Add(new ConflictError("AUSENCIA", $"{guardaNome} (equipe) possui ausência na data {data:dd/MM/yyyy}"));

                    var conflito = await _context.EscalaAlocacoes
                        .Include(ea => ea.EscalaItem)
                        .AnyAsync(ea =>
                            ea.GuardaId == membro.GuardaId
                            && ea.EscalaItem.Data == data
                            && ea.EscalaItem.HorarioId == horarioId
                            && (excludeItemId == null || ea.EscalaItemId != excludeItemId));
                    if (conflito)
                        errors.Add(new ConflictError("CONFLITO_ESCALA", $"{guardaNome} (equipe) já está escalado na data {data:dd/MM/yyyy} no mesmo horário"));
                }
            }
        }

        return errors;
    }
}
