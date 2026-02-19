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
        // 1. RET Evento precisa de evento vinculado que ocorra no mesmo mês do RET
        // Spec: "RET EVENTO e não há evento no mês? → BLOQUEAR"
        // O RET de Evento não precisa ser dentro do período do evento — apenas o evento
        // deve ocorrer (ou sobrepor) o mesmo mês do RET.
        // Evidência: CENÁRIO VÁLIDO 3 — RET EVENTO em 19/02 com Carnaval 14-17/02 → VÁLIDO ✅
        if (tipo == TipoRet.Evento)
        {
            if (!eventoId.HasValue)
                return "RET de Evento requer um evento vinculado";

            var evento = await _context.Eventos.FindAsync(eventoId.Value);
            if (evento == null)
                return "Evento não encontrado";

            // Verifica sobreposição entre o período do evento e o mês do RET
            var retMesInicio = new DateOnly(data.Year, data.Month, 1);
            var retMesFim = retMesInicio.AddMonths(1).AddDays(-1);

            if (evento.DataFim < retMesInicio || evento.DataInicio > retMesFim)
                return "O evento selecionado não ocorre no mês do RET";
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

        // 4. Descanso 12h antes - verificar última escala do guarda antes do RET
        // RET pode cruzar meia-noite (ex: 22:00-06:00), então o fim real pode ser no dia seguinte
        var retInicio = data.ToDateTime(horarioInicio);
        var retFim = horarioFim < horarioInicio
            ? data.AddDays(1).ToDateTime(horarioFim)   // RET noturno: fim no dia seguinte
            : data.ToDateTime(horarioFim);

        // Buscar alocações do guarda em janela de 3 dias antes do RET
        // para cobrir escalas noturnas (que terminam no dia seguinte)
        var windowAntes = data.AddDays(-3);
        var alocacoesAntes = await _context.EscalaAlocacoes
            .Include(a => a.EscalaItem).ThenInclude(i => i.Horario)
            .Where(a => a.GuardaId == guardaId
                        && a.EscalaItem.Data >= windowAntes
                        && a.EscalaItem.Data <= data)
            .ToListAsync();

        // Calcula o fim real de cada escala, considerando turnos noturnos (Fim < Inicio → próximo dia)
        var fimsAntes = alocacoesAntes
            .Select(a => a.EscalaItem.Horario.Fim < a.EscalaItem.Horario.Inicio
                ? a.EscalaItem.Data.AddDays(1).ToDateTime(a.EscalaItem.Horario.Fim)
                : a.EscalaItem.Data.ToDateTime(a.EscalaItem.Horario.Fim))
            .Where(fim => fim <= retInicio)
            .OrderByDescending(fim => fim)
            .ToList();

        DateTime? escalaBefore = fimsAntes.Count > 0 ? fimsAntes[0] : null;

        if (escalaBefore.HasValue)
        {
            var intervaloAntes = (retInicio - escalaBefore.Value).TotalHours;
            if (intervaloAntes < 12)
                return $"Descanso insuficiente antes do RET: {intervaloAntes:F1}h (mínimo 12h)";
        }

        // 5. Descanso 12h depois - verificar próxima escala do guarda após o RET
        var windowDepois = data.AddDays(3);
        var alocacoesDepois = await _context.EscalaAlocacoes
            .Include(a => a.EscalaItem).ThenInclude(i => i.Horario)
            .Where(a => a.GuardaId == guardaId
                        && a.EscalaItem.Data >= data
                        && a.EscalaItem.Data <= windowDepois)
            .ToListAsync();

        var iniciaisDepois = alocacoesDepois
            .Select(a => a.EscalaItem.Data.ToDateTime(a.EscalaItem.Horario.Inicio))
            .Where(inicio => inicio >= retFim)
            .OrderBy(inicio => inicio)
            .ToList();

        DateTime? escalaAfter = iniciaisDepois.Count > 0 ? iniciaisDepois[0] : null;

        if (escalaAfter.HasValue)
        {
            var intervaloDepois = (escalaAfter.Value - retFim).TotalHours;
            if (intervaloDepois < 12)
                return $"Descanso insuficiente após o RET: {intervaloDepois:F1}h (mínimo 12h)";
        }

        // 6. Intervalo total >= 32h (se há escala antes E depois)
        if (escalaBefore.HasValue && escalaAfter.HasValue)
        {
            var intervaloTotal = (escalaAfter.Value - escalaBefore.Value).TotalHours;
            if (intervaloTotal < 32)
                return $"Intervalo total entre escalas insuficiente: {intervaloTotal:F1}h (mínimo 32h)";
        }

        return null;
    }
}
