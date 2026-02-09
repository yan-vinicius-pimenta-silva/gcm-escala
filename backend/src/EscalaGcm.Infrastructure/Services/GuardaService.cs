using EscalaGcm.Application.DTOs.Guardas;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class GuardaService : IGuardaService
{
    private readonly AppDbContext _context;
    public GuardaService(AppDbContext context) => _context = context;

    public async Task<List<GuardaDto>> GetAllAsync() =>
        await _context.Guardas.Include(g => g.Posicao).OrderBy(g => g.Nome)
            .Select(g => new GuardaDto(g.Id, g.Nome, g.Telefone, g.PosicaoId, g.Posicao.Nome, g.Ativo))
            .ToListAsync();

    public async Task<GuardaDto?> GetByIdAsync(int id)
    {
        var g = await _context.Guardas.Include(g => g.Posicao).FirstOrDefaultAsync(g => g.Id == id);
        return g == null ? null : new GuardaDto(g.Id, g.Nome, g.Telefone, g.PosicaoId, g.Posicao.Nome, g.Ativo);
    }

    public async Task<GuardaDto> CreateAsync(CreateGuardaRequest request)
    {
        var entity = new Guarda { Nome = request.Nome, Telefone = request.Telefone, PosicaoId = request.PosicaoId, Ativo = request.Ativo };
        _context.Guardas.Add(entity);
        await _context.SaveChangesAsync();
        await _context.Entry(entity).Reference(e => e.Posicao).LoadAsync();
        return new GuardaDto(entity.Id, entity.Nome, entity.Telefone, entity.PosicaoId, entity.Posicao.Nome, entity.Ativo);
    }

    public async Task<GuardaDto?> UpdateAsync(int id, UpdateGuardaRequest request)
    {
        var entity = await _context.Guardas.Include(g => g.Posicao).FirstOrDefaultAsync(g => g.Id == id);
        if (entity == null) return null;
        entity.Nome = request.Nome;
        entity.Telefone = request.Telefone;
        entity.PosicaoId = request.PosicaoId;
        entity.Ativo = request.Ativo;
        await _context.SaveChangesAsync();
        await _context.Entry(entity).Reference(e => e.Posicao).LoadAsync();
        return new GuardaDto(entity.Id, entity.Nome, entity.Telefone, entity.PosicaoId, entity.Posicao.Nome, entity.Ativo);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Guardas.FindAsync(id);
        if (entity == null) return (false, "Guarda não encontrado");
        var hasAlocacoes = await _context.EscalaAlocacoes.AnyAsync(a => a.GuardaId == id);
        if (hasAlocacoes) return (false, "Não é possível excluir guarda com registros históricos");
        _context.Guardas.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
