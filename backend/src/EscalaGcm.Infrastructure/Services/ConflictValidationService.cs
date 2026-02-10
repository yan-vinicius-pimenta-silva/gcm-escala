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

        // 1. Collect every guard involved in this request (direct + team members)
        var guardaEntries = new List<(int GuardaId, string Nome)>();

        foreach (var aloc in alocacoes)
        {
            if (aloc.GuardaId.HasValue)
            {
                var guarda = await _context.Guardas.FindAsync(aloc.GuardaId.Value);
                guardaEntries.Add((aloc.GuardaId.Value, guarda?.Nome ?? $"Guarda #{aloc.GuardaId.Value}"));
            }

            if (aloc.EquipeId.HasValue)
            {
                var membros = await _context.EquipeMembros
                    .Include(m => m.Guarda)
                    .Where(m => m.EquipeId == aloc.EquipeId.Value)
                    .ToListAsync();
                foreach (var m in membros)
                    guardaEntries.Add((m.GuardaId, m.Guarda.Nome));
            }
        }

        // 2. Detect duplicate guards within the same request (e.g. same guard as Motorista + Apoio)
        var duplicates = guardaEntries.GroupBy(e => e.GuardaId).Where(g => g.Count() > 1);
        foreach (var dup in duplicates)
            errors.Add(new ConflictError("GUARDA_DUPLICADO",
                $"{dup.First().Nome} está alocado em mais de uma função na mesma escala"));

        // 3. For each unique guard, check vacation, absence, and schedule conflicts
        var checkedGuardas = new HashSet<int>();

        foreach (var aloc in alocacoes)
        {
            if (aloc.GuardaId.HasValue)
            {
                var guardaId = aloc.GuardaId.Value;
                if (!checkedGuardas.Add(guardaId)) continue;
                var guarda = await _context.Guardas.FindAsync(guardaId);
                var guardaNome = guarda?.Nome ?? $"Guarda #{guardaId}";
                await ValidateGuardaAsync(errors, guardaId, guardaNome, data, horarioId, excludeItemId);
            }

            if (aloc.EquipeId.HasValue)
            {
                var membros = await _context.EquipeMembros
                    .Include(m => m.Guarda)
                    .Where(m => m.EquipeId == aloc.EquipeId.Value)
                    .ToListAsync();

                foreach (var membro in membros)
                {
                    if (!checkedGuardas.Add(membro.GuardaId)) continue;
                    await ValidateGuardaAsync(errors, membro.GuardaId,
                        $"{membro.Guarda.Nome} (equipe)", data, horarioId, excludeItemId);
                }
            }
        }

        return errors;
    }

    private async Task ValidateGuardaAsync(List<ConflictError> errors, int guardaId, string guardaNome,
        DateOnly data, int horarioId, int? excludeItemId)
    {
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

        // Check schedule conflict — direct allocation
        var conflitoDireto = await _context.EscalaAlocacoes
            .Include(ea => ea.EscalaItem)
            .AnyAsync(ea =>
                ea.GuardaId == guardaId
                && ea.EscalaItem.Data == data
                && ea.EscalaItem.HorarioId == horarioId
                && (excludeItemId == null || ea.EscalaItemId != excludeItemId));

        // Check schedule conflict — guard is member of an already-allocated team
        var conflitoEquipe = await _context.EscalaAlocacoes
            .Include(ea => ea.EscalaItem)
            .Where(ea =>
                ea.EquipeId != null
                && ea.EscalaItem.Data == data
                && ea.EscalaItem.HorarioId == horarioId
                && (excludeItemId == null || ea.EscalaItemId != excludeItemId))
            .AnyAsync(ea => _context.EquipeMembros
                .Any(m => m.EquipeId == ea.EquipeId && m.GuardaId == guardaId));

        if (conflitoDireto || conflitoEquipe)
            errors.Add(new ConflictError("CONFLITO_ESCALA",
                $"{guardaNome} já está escalado na data {data:dd/MM/yyyy} no mesmo horário"));
    }
}
