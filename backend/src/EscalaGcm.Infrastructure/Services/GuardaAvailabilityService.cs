using EscalaGcm.Application.DTOs.Guardas;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Enums;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class GuardaAvailabilityService : IGuardaAvailabilityService
{
    private readonly AppDbContext _context;

    public GuardaAvailabilityService(AppDbContext context) => _context = context;

    public async Task<List<DayAvailabilityDto>> GetAvailabilityAsync(int guardaId, int ano, int mes)
    {
        var firstDayOfMonth = new DateOnly(ano, mes, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        // We need to look 2 days before the month start to capture shifts/RETs
        // whose occupation periods bleed into the month
        var queryStart = firstDayOfMonth.AddDays(-2);

        // 1. Fetch férias overlapping with month
        var ferias = await _context.Ferias
            .Where(f => f.GuardaId == guardaId && f.DataInicio <= lastDayOfMonth && f.DataFim >= firstDayOfMonth)
            .ToListAsync();

        // 2. Fetch ausências overlapping with month
        var ausencias = await _context.Ausencias
            .Where(a => a.GuardaId == guardaId && a.DataInicio <= lastDayOfMonth && a.DataFim >= firstDayOfMonth)
            .ToListAsync();

        // 3. Fetch escala items where the guard is directly allocated
        var directItems = await _context.EscalaAlocacoes
            .Include(ea => ea.EscalaItem).ThenInclude(i => i.Horario)
            .Where(ea =>
                ea.GuardaId == guardaId
                && ea.EscalaItem.Data >= queryStart
                && ea.EscalaItem.Data <= lastDayOfMonth)
            .Select(ea => ea.EscalaItem)
            .Distinct()
            .ToListAsync();

        // 4. Fetch escala items where the guard is allocated via team
        var teamItems = await _context.EscalaAlocacoes
            .Include(ea => ea.EscalaItem).ThenInclude(i => i.Horario)
            .Where(ea =>
                ea.EquipeId != null
                && ea.EscalaItem.Data >= queryStart
                && ea.EscalaItem.Data <= lastDayOfMonth
                && _context.EquipeMembros.Any(m => m.EquipeId == ea.EquipeId && m.GuardaId == guardaId))
            .Select(ea => ea.EscalaItem)
            .Distinct()
            .ToListAsync();

        var allItems = directItems.Union(teamItems).DistinctBy(i => i.Id).ToList();

        // 5. Fetch RETs for this guard
        var rets = await _context.Rets
            .Where(r => r.GuardaId == guardaId && r.Data >= queryStart && r.Data <= lastDayOfMonth)
            .ToListAsync();

        // Build occupation intervals grouped by day for the target month
        var result = new List<DayAvailabilityDto>();

        for (int day = 1; day <= lastDayOfMonth.Day; day++)
        {
            var dayStart = new DateTime(ano, mes, day, 0, 0, 0);
            var dayEnd = dayStart.AddDays(1);

            var intervals = new List<(DateTime From, DateTime To, string TipoMotivo)>();

            // Add férias intervals
            foreach (var f in ferias)
            {
                var fFrom = f.DataInicio.ToDateTime(TimeOnly.MinValue);
                var fTo = f.DataFim.ToDateTime(TimeOnly.MinValue).AddDays(1); // DataFim is inclusive
                if (fFrom < dayEnd && fTo > dayStart)
                    intervals.Add((Max(fFrom, dayStart), Min(fTo, dayEnd), "FERIAS"));
            }

            // Add ausência intervals
            foreach (var a in ausencias)
            {
                var aFrom = a.DataInicio.ToDateTime(TimeOnly.MinValue);
                var aTo = a.DataFim.ToDateTime(TimeOnly.MinValue).AddDays(1);
                if (aFrom < dayEnd && aTo > dayStart)
                    intervals.Add((Max(aFrom, dayStart), Min(aTo, dayEnd), "AUSENCIA"));
            }

            // Add escala item occupation intervals
            foreach (var item in allItems)
            {
                var (shiftStart, occupiedTo, tipoMotivo) = ComputeOccupation(item);
                if (shiftStart < dayEnd && occupiedTo > dayStart)
                    intervals.Add((Max(shiftStart, dayStart), Min(occupiedTo, dayEnd), tipoMotivo));
            }

            // Add RET occupation intervals (32h rest after RET)
            foreach (var ret in rets)
            {
                var retStart = ret.Data.ToDateTime(ret.HorarioInicio);
                DateTime retEnd;
                if (ret.HorarioFim > ret.HorarioInicio)
                    retEnd = ret.Data.ToDateTime(ret.HorarioFim);
                else
                    retEnd = ret.Data.ToDateTime(TimeOnly.MinValue).AddDays(1).Add(ret.HorarioFim.ToTimeSpan());

                var retRestEnd = retEnd.AddHours(32);
                if (retStart < dayEnd && retRestEnd > dayStart)
                    intervals.Add((Max(retStart, dayStart), Min(retRestEnd, dayEnd), "RET"));
            }

            var dayAvailability = ComputeDayStatus(day, intervals, dayStart, dayEnd);
            result.Add(dayAvailability);
        }

        return result;
    }

    private static (DateTime ShiftStart, DateTime OccupiedTo, string TipoMotivo) ComputeOccupation(Domain.Entities.EscalaItem item)
    {
        var horario = item.Horario;
        var shiftStart = item.Data.ToDateTime(horario.Inicio);

        DateTime shiftEnd;
        if (horario.Fim > horario.Inicio)
            shiftEnd = item.Data.ToDateTime(horario.Fim);
        else
            shiftEnd = item.Data.ToDateTime(TimeOnly.MinValue).AddDays(1).Add(horario.Fim.ToTimeSpan());

        DateTime occupiedTo;
        string tipoMotivo;
        if (item.Regime == RegimeTrabalho.Doze36)
        {
            occupiedTo = shiftEnd.AddHours(36);
            tipoMotivo = "FOLGA_12X36";
        }
        else
        {
            occupiedTo = shiftEnd;
            tipoMotivo = "ESCALA_EXISTENTE";
        }

        return (shiftStart, occupiedTo, tipoMotivo);
    }

    private static DayAvailabilityDto ComputeDayStatus(int day, List<(DateTime From, DateTime To, string TipoMotivo)> intervals, DateTime dayStart, DateTime dayEnd)
    {
        if (intervals.Count == 0)
            return new DayAvailabilityDto(day, StatusDisponibilidade.Disponivel, null, null, null);

        // Sort by start time
        var sorted = intervals.OrderBy(i => i.From).ToList();

        // Find first free moment starting from 00:00
        var currentEnd = dayStart;
        string? blockingTipo = null;

        foreach (var (from, to, tipo) in sorted)
        {
            if (from > currentEnd)
                break; // Gap found — guard is free from currentEnd

            if (to > currentEnd)
            {
                currentEnd = to;
                blockingTipo ??= tipo;
            }
        }

        if (currentEnd >= dayEnd)
        {
            // Fully blocked
            var primaryTipo = sorted.First().TipoMotivo;
            return new DayAvailabilityDto(day, StatusDisponibilidade.Bloqueado, null,
                BuildMotivoMessage(primaryTipo, sorted.First().From), primaryTipo);
        }

        if (currentEnd == dayStart)
        {
            // Free from the start
            return new DayAvailabilityDto(day, StatusDisponibilidade.Disponivel, null, null, null);
        }

        // Partially occupied at start
        var freeFrom = currentEnd.ToString("HH:mm");
        return new DayAvailabilityDto(day, StatusDisponibilidade.Parcial, freeFrom,
            $"Disponível a partir das {freeFrom}", blockingTipo);
    }

    private static string BuildMotivoMessage(string tipo, DateTime occupiedFrom) => tipo switch
    {
        "FERIAS" => "Guarda em período de férias",
        "AUSENCIA" => "Guarda com ausência registrada",
        "FOLGA_12X36" => "Guarda em período de folga obrigatória (12x36)",
        "RET" => "Guarda em descanso obrigatório após RET",
        _ => "Guarda indisponível"
    };

    private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;
    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
}
