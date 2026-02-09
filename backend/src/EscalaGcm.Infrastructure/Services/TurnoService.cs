using EscalaGcm.Application.DTOs.Turnos;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class TurnoService : ITurnoService
{
    private readonly AppDbContext _context;
    public TurnoService(AppDbContext context) => _context = context;

    public async Task<List<TurnoDto>> GetAllAsync() =>
        await _context.Turnos.OrderBy(t => t.Nome)
            .Select(t => new TurnoDto(t.Id, t.Nome, t.Ativo)).ToListAsync();

    public async Task<TurnoDto?> GetByIdAsync(int id)
    {
        var t = await _context.Turnos.FindAsync(id);
        return t == null ? null : new TurnoDto(t.Id, t.Nome, t.Ativo);
    }

    public async Task<TurnoDto> CreateAsync(CreateTurnoRequest request)
    {
        var entity = new Turno { Nome = request.Nome, Ativo = request.Ativo };
        _context.Turnos.Add(entity);
        await _context.SaveChangesAsync();
        return new TurnoDto(entity.Id, entity.Nome, entity.Ativo);
    }

    public async Task<TurnoDto?> UpdateAsync(int id, UpdateTurnoRequest request)
    {
        var entity = await _context.Turnos.FindAsync(id);
        if (entity == null) return null;
        entity.Nome = request.Nome;
        entity.Ativo = request.Ativo;
        await _context.SaveChangesAsync();
        return new TurnoDto(entity.Id, entity.Nome, entity.Ativo);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Turnos.FindAsync(id);
        if (entity == null) return (false, "Turno não encontrado");
        var hasEscalaItens = await _context.EscalaItens.AnyAsync(ei => ei.TurnoId == id);
        if (hasEscalaItens) return (false, "Não é possível excluir turno em uso em escalas");
        _context.Turnos.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
