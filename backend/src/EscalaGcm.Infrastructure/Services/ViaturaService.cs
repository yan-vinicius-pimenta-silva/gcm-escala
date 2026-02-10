using EscalaGcm.Application.DTOs.Viaturas;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class ViaturaService : IViaturaService
{
    private readonly AppDbContext _context;
    public ViaturaService(AppDbContext context) => _context = context;

    public async Task<List<ViaturaDto>> GetAllAsync() =>
        await _context.Viaturas.OrderBy(v => v.Identificador)
            .Select(v => new ViaturaDto(v.Id, v.Identificador, v.Ativo)).ToListAsync();

    public async Task<ViaturaDto?> GetByIdAsync(int id)
    {
        var v = await _context.Viaturas.FindAsync(id);
        return v == null ? null : new ViaturaDto(v.Id, v.Identificador, v.Ativo);
    }

    public async Task<ViaturaDto> CreateAsync(CreateViaturaRequest request)
    {
        var entity = new Viatura { Identificador = request.Identificador, Ativo = request.Ativo };
        _context.Viaturas.Add(entity);
        await _context.SaveChangesAsync();
        return new ViaturaDto(entity.Id, entity.Identificador, entity.Ativo);
    }

    public async Task<ViaturaDto?> UpdateAsync(int id, UpdateViaturaRequest request)
    {
        var entity = await _context.Viaturas.FindAsync(id);
        if (entity == null) return null;
        entity.Identificador = request.Identificador;
        entity.Ativo = request.Ativo;
        await _context.SaveChangesAsync();
        return new ViaturaDto(entity.Id, entity.Identificador, entity.Ativo);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Viaturas.FindAsync(id);
        if (entity == null) return (false, "Viatura não encontrada");
        var hasAlocacoes = await _context.EscalaAlocacoes.AnyAsync(a => a.ViaturaId == id);
        if (hasAlocacoes) return (false, "Não é possível excluir viatura em uso em escalas");
        _context.Viaturas.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
