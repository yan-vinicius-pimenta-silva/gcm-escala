using EscalaGcm.Application.DTOs.Horarios;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class HorarioService : IHorarioService
{
    private readonly AppDbContext _context;
    public HorarioService(AppDbContext context) => _context = context;

    public async Task<List<HorarioDto>> GetAllAsync() =>
        await _context.Horarios.OrderBy(h => h.Inicio)
            .Select(h => new HorarioDto(h.Id, h.Inicio.ToString("HH:mm"), h.Fim.ToString("HH:mm"), h.Descricao, h.Ativo))
            .ToListAsync();

    public async Task<HorarioDto?> GetByIdAsync(int id)
    {
        var h = await _context.Horarios.FindAsync(id);
        return h == null ? null : new HorarioDto(h.Id, h.Inicio.ToString("HH:mm"), h.Fim.ToString("HH:mm"), h.Descricao, h.Ativo);
    }

    public async Task<HorarioDto> CreateAsync(CreateHorarioRequest request)
    {
        var inicio = TimeOnly.Parse(request.Inicio);
        var fim = TimeOnly.Parse(request.Fim);
        var descricao = request.Descricao ?? $"{inicio:HH:mm} às {fim:HH:mm}";
        var entity = new Horario { Inicio = inicio, Fim = fim, Descricao = descricao, Ativo = request.Ativo };
        _context.Horarios.Add(entity);
        await _context.SaveChangesAsync();
        return new HorarioDto(entity.Id, entity.Inicio.ToString("HH:mm"), entity.Fim.ToString("HH:mm"), entity.Descricao, entity.Ativo);
    }

    public async Task<HorarioDto?> UpdateAsync(int id, UpdateHorarioRequest request)
    {
        var entity = await _context.Horarios.FindAsync(id);
        if (entity == null) return null;
        entity.Inicio = TimeOnly.Parse(request.Inicio);
        entity.Fim = TimeOnly.Parse(request.Fim);
        entity.Descricao = request.Descricao ?? $"{entity.Inicio:HH:mm} às {entity.Fim:HH:mm}";
        entity.Ativo = request.Ativo;
        await _context.SaveChangesAsync();
        return new HorarioDto(entity.Id, entity.Inicio.ToString("HH:mm"), entity.Fim.ToString("HH:mm"), entity.Descricao, entity.Ativo);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Horarios.FindAsync(id);
        if (entity == null) return (false, "Horário não encontrado");
        var hasEscalaItens = await _context.EscalaItens.AnyAsync(ei => ei.HorarioId == id);
        if (hasEscalaItens) return (false, "Não é possível excluir horário em uso em escalas");
        _context.Horarios.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
