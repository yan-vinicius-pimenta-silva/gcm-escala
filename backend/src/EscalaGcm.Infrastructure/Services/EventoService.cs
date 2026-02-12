using EscalaGcm.Application.DTOs.Eventos;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class EventoService : IEventoService
{
    private readonly AppDbContext _context;
    public EventoService(AppDbContext context) => _context = context;

    public async Task<List<EventoDto>> GetAllAsync() =>
        await _context.Eventos.OrderByDescending(e => e.DataInicio)
            .Select(e => new EventoDto(e.Id, e.Nome,
                e.DataInicio.ToString("yyyy-MM-dd"), e.DataFim.ToString("yyyy-MM-dd")))
            .ToListAsync();

    public async Task<EventoDto?> GetByIdAsync(int id)
    {
        var e = await _context.Eventos.FindAsync(id);
        if (e == null) return null;
        return new EventoDto(e.Id, e.Nome,
            e.DataInicio.ToString("yyyy-MM-dd"), e.DataFim.ToString("yyyy-MM-dd"));
    }

    public async Task<(EventoDto? Result, string? Error)> CreateAsync(CreateEventoRequest request)
    {
        var inicio = DateOnly.Parse(request.DataInicio);
        var fim = DateOnly.Parse(request.DataFim);
        if (fim < inicio) return (null, "Data fim deve ser maior ou igual à data início");

        var overlap = await _context.Eventos.AnyAsync(e =>
            e.DataInicio <= fim && e.DataFim >= inicio);
        if (overlap) return (null, "Já existe evento cadastrado neste período");

        var entity = new Evento { Nome = request.Nome, DataInicio = inicio, DataFim = fim };
        _context.Eventos.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(EventoDto? Result, string? Error)> UpdateAsync(int id, UpdateEventoRequest request)
    {
        var entity = await _context.Eventos.FindAsync(id);
        if (entity == null) return (null, "Evento não encontrado");

        var inicio = DateOnly.Parse(request.DataInicio);
        var fim = DateOnly.Parse(request.DataFim);
        if (fim < inicio) return (null, "Data fim deve ser maior ou igual à data início");

        var overlap = await _context.Eventos.AnyAsync(e =>
            e.Id != id && e.DataInicio <= fim && e.DataFim >= inicio);
        if (overlap) return (null, "Já existe evento cadastrado neste período");

        entity.Nome = request.Nome;
        entity.DataInicio = inicio;
        entity.DataFim = fim;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Eventos.FindAsync(id);
        if (entity == null) return (false, "Evento não encontrado");

        var hasRets = await _context.Rets.AnyAsync(r => r.EventoId == id);
        if (hasRets) return (false, "Não é possível excluir evento com RETs vinculados");

        _context.Eventos.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
