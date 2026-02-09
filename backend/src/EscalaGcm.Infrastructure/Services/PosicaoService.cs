using EscalaGcm.Application.DTOs.Posicoes;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class PosicaoService : IPosicaoService
{
    private readonly AppDbContext _context;
    public PosicaoService(AppDbContext context) => _context = context;

    public async Task<List<PosicaoDto>> GetAllAsync() =>
        await _context.Posicoes.OrderBy(p => p.Nome)
            .Select(p => new PosicaoDto(p.Id, p.Nome, p.Ativo)).ToListAsync();

    public async Task<PosicaoDto?> GetByIdAsync(int id)
    {
        var p = await _context.Posicoes.FindAsync(id);
        return p == null ? null : new PosicaoDto(p.Id, p.Nome, p.Ativo);
    }

    public async Task<PosicaoDto> CreateAsync(CreatePosicaoRequest request)
    {
        var entity = new Posicao { Nome = request.Nome, Ativo = request.Ativo };
        _context.Posicoes.Add(entity);
        await _context.SaveChangesAsync();
        return new PosicaoDto(entity.Id, entity.Nome, entity.Ativo);
    }

    public async Task<PosicaoDto?> UpdateAsync(int id, UpdatePosicaoRequest request)
    {
        var entity = await _context.Posicoes.FindAsync(id);
        if (entity == null) return null;
        entity.Nome = request.Nome;
        entity.Ativo = request.Ativo;
        await _context.SaveChangesAsync();
        return new PosicaoDto(entity.Id, entity.Nome, entity.Ativo);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Posicoes.FindAsync(id);
        if (entity == null) return (false, "Posição não encontrada");
        var hasGuardas = await _context.Guardas.AnyAsync(g => g.PosicaoId == id);
        if (hasGuardas) return (false, "Não é possível excluir posição em uso por guardas");
        _context.Posicoes.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
