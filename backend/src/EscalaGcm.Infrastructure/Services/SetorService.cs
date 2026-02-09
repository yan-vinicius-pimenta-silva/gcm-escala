using EscalaGcm.Application.DTOs.Setores;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class SetorService : ISetorService
{
    private readonly AppDbContext _context;
    public SetorService(AppDbContext context) => _context = context;

    public async Task<List<SetorDto>> GetAllAsync() =>
        await _context.Setores.OrderBy(s => s.Nome)
            .Select(s => new SetorDto(s.Id, s.Nome, s.Tipo, s.Ativo)).ToListAsync();

    public async Task<SetorDto?> GetByIdAsync(int id)
    {
        var s = await _context.Setores.FindAsync(id);
        return s == null ? null : new SetorDto(s.Id, s.Nome, s.Tipo, s.Ativo);
    }

    public async Task<SetorDto> CreateAsync(CreateSetorRequest request)
    {
        var entity = new Setor { Nome = request.Nome, Tipo = request.Tipo, Ativo = request.Ativo };
        _context.Setores.Add(entity);
        await _context.SaveChangesAsync();
        return new SetorDto(entity.Id, entity.Nome, entity.Tipo, entity.Ativo);
    }

    public async Task<SetorDto?> UpdateAsync(int id, UpdateSetorRequest request)
    {
        var entity = await _context.Setores.FindAsync(id);
        if (entity == null) return null;
        entity.Nome = request.Nome;
        entity.Tipo = request.Tipo;
        entity.Ativo = request.Ativo;
        await _context.SaveChangesAsync();
        return new SetorDto(entity.Id, entity.Nome, entity.Tipo, entity.Ativo);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Setores.FindAsync(id);
        if (entity == null) return (false, "Setor não encontrado");
        var hasEscalas = await _context.Escalas.AnyAsync(e => e.SetorId == id);
        if (hasEscalas) return (false, "Não é possível excluir setor com escalas vinculadas");
        _context.Setores.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
