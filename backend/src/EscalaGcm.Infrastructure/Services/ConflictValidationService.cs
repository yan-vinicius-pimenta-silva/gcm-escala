using EscalaGcm.Application.DTOs.Escalas;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Enums;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class ConflictValidationService : IConflictValidationService
{
    private readonly AppDbContext _context;
    public ConflictValidationService(AppDbContext context) => _context = context;

    public async Task<List<ConflictError>> ValidateAllocationsAsync(DateOnly data, int horarioId, List<AlocacaoRequest> alocacoes, int? excludeItemId = null, RegimeTrabalho regime = RegimeTrabalho.Doze36)
    {
        var errors = new List<ConflictError>();

        // 1. Collect every guard involved in this request (direct + team members)
        // REVIEW: N+1 query problem. Each allocation triggers individual DB calls (FindAsync per guard,
        // query per team). Then step 3 re-queries the same teams again. Batch-load all guards and
        // team members upfront in a single query to reduce round-trips.
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
                await ValidateGuardaAsync(errors, guardaId, guardaNome, data, horarioId, excludeItemId, regime);
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
                        $"{membro.Guarda.Nome} (equipe)", data, horarioId, excludeItemId, regime);
                }
            }
        }

        return errors;
    }

    private async Task ValidateGuardaAsync(List<ConflictError> errors, int guardaId, string guardaNome,
        DateOnly data, int horarioId, int? excludeItemId, RegimeTrabalho regime)
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

        // Get the target horario for time-overlap checks
        var targetHorario = await _context.Horarios.FindAsync(horarioId);
        if (targetHorario != null)
        {
            var targetDate = data.ToDateTime(TimeOnly.MinValue);
            var targetStart = targetDate.Add(targetHorario.Inicio.ToTimeSpan());
            DateTime targetEnd;
            if (targetHorario.Fim > targetHorario.Inicio)
                targetEnd = targetDate.Add(targetHorario.Fim.ToTimeSpan());
            else
                targetEnd = targetDate.AddDays(1).Add(targetHorario.Fim.ToTimeSpan());

            // Check 12x36 rest period: any existing 12x36 allocation whose rest period covers the new shift
            var existingItems = await _context.EscalaAlocacoes
                .Include(ea => ea.EscalaItem).ThenInclude(i => i.Horario)
                .Where(ea =>
                    ea.GuardaId == guardaId
                    && ea.EscalaItem.Regime == RegimeTrabalho.Doze36
                    && (excludeItemId == null || ea.EscalaItemId != excludeItemId))
                .Select(ea => ea.EscalaItem)
                .ToListAsync();

            var teamItems = await _context.EscalaAlocacoes
                .Include(ea => ea.EscalaItem).ThenInclude(i => i.Horario)
                .Where(ea =>
                    ea.EquipeId != null
                    && ea.EscalaItem.Regime == RegimeTrabalho.Doze36
                    && (excludeItemId == null || ea.EscalaItemId != excludeItemId)
                    && _context.EquipeMembros.Any(m => m.EquipeId == ea.EquipeId && m.GuardaId == guardaId))
                .Select(ea => ea.EscalaItem)
                .ToListAsync();

            foreach (var existing in existingItems.Union(teamItems).DistinctBy(i => i.Id))
            {
                var existingDate = existing.Data.ToDateTime(TimeOnly.MinValue);
                var existingStart = existingDate.Add(existing.Horario.Inicio.ToTimeSpan());
                DateTime existingShiftEnd;
                if (existing.Horario.Fim > existing.Horario.Inicio)
                    existingShiftEnd = existingDate.Add(existing.Horario.Fim.ToTimeSpan());
                else
                    existingShiftEnd = existingDate.AddDays(1).Add(existing.Horario.Fim.ToTimeSpan());

                var existingRestEnd = existingShiftEnd.AddHours(36);

                // If the new shift starts before the rest period ends, it's a conflict
                if (targetStart < existingRestEnd && targetEnd > existingStart)
                {
                    errors.Add(new ConflictError("FOLGA_12X36",
                        $"{guardaNome} está em período de folga obrigatória (12x36) na data {data:dd/MM/yyyy}. " +
                        $"Trabalhou em {existing.Data:dd/MM/yyyy}, folga até {existingRestEnd:dd/MM/yyyy HH:mm}"));
                    break;
                }
            }

            // Check RET rest period (32h after RET)
            var rets = await _context.Rets
                .Where(r => r.GuardaId == guardaId)
                .ToListAsync();

            foreach (var ret in rets)
            {
                var retDate = ret.Data.ToDateTime(TimeOnly.MinValue);
                var retStart = retDate.Add(ret.HorarioInicio.ToTimeSpan());
                DateTime retEnd;
                if (ret.HorarioFim > ret.HorarioInicio)
                    retEnd = retDate.Add(ret.HorarioFim.ToTimeSpan());
                else
                    retEnd = retDate.AddDays(1).Add(ret.HorarioFim.ToTimeSpan());

                var retRestEnd = retEnd.AddHours(32);

                if (targetStart < retRestEnd && targetEnd > retStart)
                {
                    errors.Add(new ConflictError("RET",
                        $"{guardaNome} está em descanso obrigatório após RET em {ret.Data:dd/MM/yyyy}. " +
                        $"Disponível a partir de {retRestEnd:dd/MM/yyyy HH:mm}"));
                    break;
                }
            }
        }

        // Check schedule conflict — direct allocation (same date, same horario)
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
