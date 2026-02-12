using EscalaGcm.Application.DTOs.Rets;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Domain.Enums;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class RetService : IRetService
{
    private readonly AppDbContext _context;
    public RetService(AppDbContext context) => _context = context;

    public async Task<List<RetDto>> GetAllAsync(int? guardaId = null, int? mes = null, int? ano = null)
    {
        var query = _context.Rets.Include(r => r.Guarda).Include(r => r.Evento).AsQueryable();
        if (guardaId.HasValue) query = query.Where(r => r.GuardaId == guardaId.Value);
        if (mes.HasValue) query = query.Where(r => r.Data.Month == mes.Value);
        if (ano.HasValue) query = query.Where(r => r.Data.Year == ano.Value);

        return await query.OrderByDescending(r => r.Data).ThenBy(r => r.HorarioInicio)
            .Select(r => new RetDto(r.Id, r.GuardaId, r.Guarda.Nome,
                r.Data.ToString("yyyy-MM-dd"),
                r.HorarioInicio.ToString("HH:mm"),
                r.HorarioFim.ToString("HH:mm"),
                r.TipoRet, r.EventoId, r.Evento != null ? r.Evento.Nome : null, r.Observacao))
            .ToListAsync();
    }

    public async Task<RetDto?> GetByIdAsync(int id)
    {
        var r = await _context.Rets.Include(r => r.Guarda).Include(r => r.Evento)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (r == null) return null;
        return new RetDto(r.Id, r.GuardaId, r.Guarda.Nome,
            r.Data.ToString("yyyy-MM-dd"),
            r.HorarioInicio.ToString("HH:mm"),
            r.HorarioFim.ToString("HH:mm"),
            r.TipoRet, r.EventoId, r.Evento?.Nome, r.Observacao);
    }

    public async Task<(RetDto? Result, string? Error)> CreateAsync(CreateRetRequest request)
    {
        var data = DateOnly.Parse(request.Data);
        var horarioInicio = TimeOnly.Parse(request.HorarioInicio);
        var horarioFim = horarioInicio.AddHours(8);

        var error = await ValidateAsync(request.GuardaId, data, horarioInicio, horarioFim, request.Tipo, request.EventoId, null);
        if (error != null) return (null, error);

        var entity = new Ret
        {
            GuardaId = request.GuardaId,
            Data = data,
            HorarioInicio = horarioInicio,
            HorarioFim = horarioFim,
            TipoRet = request.Tipo,
            EventoId = request.EventoId,
            Observacao = request.Observacao
        };
        _context.Rets.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(RetDto? Result, string? Error)> UpdateAsync(int id, UpdateRetRequest request)
    {
        var entity = await _context.Rets.FindAsync(id);
        if (entity == null) return (null, "RET não encontrado");

        var data = DateOnly.Parse(request.Data);
        var horarioInicio = TimeOnly.Parse(request.HorarioInicio);
        var horarioFim = horarioInicio.AddHours(8);

        var error = await ValidateAsync(request.GuardaId, data, horarioInicio, horarioFim, request.Tipo, request.EventoId, id);
        if (error != null) return (null, error);

        entity.GuardaId = request.GuardaId;
        entity.Data = data;
        entity.HorarioInicio = horarioInicio;
        entity.HorarioFim = horarioFim;
        entity.TipoRet = request.Tipo;
        entity.EventoId = request.EventoId;
        entity.Observacao = request.Observacao;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Rets.FindAsync(id);
        if (entity == null) return (false, "RET não encontrado");
        _context.Rets.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private async Task<string?> ValidateAsync(int guardaId, DateOnly data, TimeOnly horarioInicio, TimeOnly horarioFim, TipoRet tipo, int? eventoId, int? excludeId)
    {
        // 1. RET Evento precisa de evento válido no período
        if (tipo == TipoRet.Evento)
        {
            if (!eventoId.HasValue)
                return "RET de Evento requer um evento vinculado";

            var evento = await _context.Eventos.FindAsync(eventoId.Value);
            if (evento == null)
                return "Evento não encontrado";

            if (data < evento.DataInicio || data > evento.DataFim)
                return "A data do RET deve estar dentro do período do evento";
        }

        // 2. Máx 1 RET Mensal por mês
        if (tipo == TipoRet.Mensal)
        {
            var countMensal = await _context.Rets.CountAsync(r =>
                r.GuardaId == guardaId && r.TipoRet == TipoRet.Mensal
                && r.Data.Month == data.Month && r.Data.Year == data.Year
                && (excludeId == null || r.Id != excludeId));
            if (countMensal >= 1)
                return "O guarda já possui 1 RET Mensal neste mês";
        }

        // 3. Máx 1 RET Evento por mês
        if (tipo == TipoRet.Evento)
        {
            var countEvento = await _context.Rets.CountAsync(r =>
                r.GuardaId == guardaId && r.TipoRet == TipoRet.Evento
                && r.Data.Month == data.Month && r.Data.Year == data.Year
                && (excludeId == null || r.Id != excludeId));
            if (countEvento >= 1)
                return "O guarda já possui 1 RET de Evento neste mês";
        }

        // 4. Sem 2 RETs na mesma semana (domingo a sábado)
        var dayOfWeek = (int)data.DayOfWeek; // Sunday = 0
        var weekStart = data.AddDays(-dayOfWeek);
        var weekEnd = weekStart.AddDays(6);
        var countWeek = await _context.Rets.CountAsync(r =>
            r.GuardaId == guardaId
            && r.Data >= weekStart && r.Data <= weekEnd
            && (excludeId == null || r.Id != excludeId));
        if (countWeek >= 1)
            return "O guarda já possui um RET nesta semana (domingo a sábado)";

        // 5. Descanso 12h antes - verificar última escala do guarda antes do RET
        var retInicio = data.ToDateTime(horarioInicio);
        var retFim = data.ToDateTime(horarioFim);

        // Buscar alocações do guarda em escalas, com horários
        var escalaBefore = await _context.EscalaAlocacoes
            .Include(a => a.EscalaItem).ThenInclude(i => i.Horario)
            .Where(a => a.GuardaId == guardaId)
            .Select(a => new
            {
                Fim = a.EscalaItem.Data.ToDateTime(a.EscalaItem.Horario.Fim)
            })
            .Where(x => x.Fim <= retInicio)
            .OrderByDescending(x => x.Fim)
            .FirstOrDefaultAsync();

        if (escalaBefore != null)
        {
            var intervaloAntes = (retInicio - escalaBefore.Fim).TotalHours;
            if (intervaloAntes < 12)
                return $"Descanso insuficiente antes do RET: {intervaloAntes:F1}h (mínimo 12h)";
        }

        // 6. Descanso 12h depois - verificar próxima escala do guarda após o RET
        var escalaAfter = await _context.EscalaAlocacoes
            .Include(a => a.EscalaItem).ThenInclude(i => i.Horario)
            .Where(a => a.GuardaId == guardaId)
            .Select(a => new
            {
                Inicio = a.EscalaItem.Data.ToDateTime(a.EscalaItem.Horario.Inicio)
            })
            .Where(x => x.Inicio >= retFim)
            .OrderBy(x => x.Inicio)
            .FirstOrDefaultAsync();

        if (escalaAfter != null)
        {
            var intervaloDepois = (escalaAfter.Inicio - retFim).TotalHours;
            if (intervaloDepois < 12)
                return $"Descanso insuficiente após o RET: {intervaloDepois:F1}h (mínimo 12h)";
        }

        // 7. Intervalo total >= 32h (se há escala antes E depois)
        if (escalaBefore != null && escalaAfter != null)
        {
            var intervaloTotal = (escalaAfter.Inicio - escalaBefore.Fim).TotalHours;
            if (intervaloTotal < 32)
                return $"Intervalo total entre escalas insuficiente: {intervaloTotal:F1}h (mínimo 32h)";
        }

        return null;
    }
}
