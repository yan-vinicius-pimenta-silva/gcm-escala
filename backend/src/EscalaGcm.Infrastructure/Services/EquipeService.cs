using EscalaGcm.Application.DTOs.Equipes;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class EquipeService : IEquipeService
{
    private readonly AppDbContext _context;
    public EquipeService(AppDbContext context) => _context = context;

    public async Task<List<EquipeDto>> GetAllAsync() =>
        await _context.Equipes.Include(e => e.Membros).ThenInclude(m => m.Guarda)
            .OrderBy(e => e.Nome)
            .Select(e => new EquipeDto(e.Id, e.Nome, e.Ativo,
                e.Membros.Select(m => new EquipeMembroDto(m.Id, m.GuardaId, m.Guarda.Nome)).ToList()))
            .ToListAsync();

    public async Task<EquipeDto?> GetByIdAsync(int id)
    {
        var e = await _context.Equipes.Include(e => e.Membros).ThenInclude(m => m.Guarda)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (e == null) return null;
        return new EquipeDto(e.Id, e.Nome, e.Ativo,
            e.Membros.Select(m => new EquipeMembroDto(m.Id, m.GuardaId, m.Guarda.Nome)).ToList());
    }

    public async Task<(EquipeDto? Result, string? Error)> CreateAsync(CreateEquipeRequest request)
    {
        if (request.GuardaIds.Count < 2 || request.GuardaIds.Count > 4)
            return (null, "Equipe deve ter entre 2 e 4 membros");

        var entity = new Equipe { Nome = request.Nome, Ativo = request.Ativo };
        _context.Equipes.Add(entity);
        await _context.SaveChangesAsync();

        foreach (var guardaId in request.GuardaIds)
            _context.EquipeMembros.Add(new EquipeMembro { EquipeId = entity.Id, GuardaId = guardaId });
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(EquipeDto? Result, string? Error)> UpdateAsync(int id, UpdateEquipeRequest request)
    {
        if (request.GuardaIds.Count < 2 || request.GuardaIds.Count > 4)
            return (null, "Equipe deve ter entre 2 e 4 membros");

        var entity = await _context.Equipes.Include(e => e.Membros).FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null) return (null, "Equipe não encontrada");

        entity.Nome = request.Nome;
        entity.Ativo = request.Ativo;

        _context.EquipeMembros.RemoveRange(entity.Membros);
        foreach (var guardaId in request.GuardaIds)
            _context.EquipeMembros.Add(new EquipeMembro { EquipeId = entity.Id, GuardaId = guardaId });
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Equipes.Include(e => e.Membros).FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null) return (false, "Equipe não encontrada");
        var hasAlocacoes = await _context.EscalaAlocacoes.AnyAsync(a => a.EquipeId == id);
        if (hasAlocacoes) return (false, "Não é possível excluir equipe em uso em escalas");
        _context.EquipeMembros.RemoveRange(entity.Membros);
        _context.Equipes.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
