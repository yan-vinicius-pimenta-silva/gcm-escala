using EscalaGcm.Application.DTOs.Ferias;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class FeriasService : IFeriasService
{
    private readonly AppDbContext _context;
    public FeriasService(AppDbContext context) => _context = context;

    public async Task<List<FeriasDto>> GetAllAsync() =>
        await _context.Ferias.Include(f => f.Guarda).OrderByDescending(f => f.DataInicio)
            .Select(f => new FeriasDto(f.Id, f.GuardaId, f.Guarda.Nome,
                f.DataInicio.ToString("yyyy-MM-dd"), f.DataFim.ToString("yyyy-MM-dd"), f.Observacao))
            .ToListAsync();

    public async Task<FeriasDto?> GetByIdAsync(int id)
    {
        var f = await _context.Ferias.Include(f => f.Guarda).FirstOrDefaultAsync(f => f.Id == id);
        if (f == null) return null;
        return new FeriasDto(f.Id, f.GuardaId, f.Guarda.Nome,
            f.DataInicio.ToString("yyyy-MM-dd"), f.DataFim.ToString("yyyy-MM-dd"), f.Observacao);
    }

    public async Task<(FeriasDto? Result, string? Error)> CreateAsync(CreateFeriasRequest request)
    {
        var inicio = DateOnly.Parse(request.DataInicio);
        var fim = DateOnly.Parse(request.DataFim);
        if (fim < inicio) return (null, "Data fim deve ser maior ou igual à data início");

        var overlap = await _context.Ferias.AnyAsync(f =>
            f.GuardaId == request.GuardaId && f.DataInicio <= fim && f.DataFim >= inicio);
        if (overlap) return (null, "Já existe férias cadastrada neste período para este guarda");

        var entity = new Ferias { GuardaId = request.GuardaId, DataInicio = inicio, DataFim = fim, Observacao = request.Observacao };
        _context.Ferias.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(FeriasDto? Result, string? Error)> UpdateAsync(int id, UpdateFeriasRequest request)
    {
        var entity = await _context.Ferias.FindAsync(id);
        if (entity == null) return (null, "Férias não encontrada");

        var inicio = DateOnly.Parse(request.DataInicio);
        var fim = DateOnly.Parse(request.DataFim);
        if (fim < inicio) return (null, "Data fim deve ser maior ou igual à data início");

        var overlap = await _context.Ferias.AnyAsync(f =>
            f.GuardaId == request.GuardaId && f.Id != id && f.DataInicio <= fim && f.DataFim >= inicio);
        if (overlap) return (null, "Já existe férias cadastrada neste período para este guarda");

        entity.GuardaId = request.GuardaId;
        entity.DataInicio = inicio;
        entity.DataFim = fim;
        entity.Observacao = request.Observacao;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Ferias.FindAsync(id);
        if (entity == null) return (false, "Férias não encontrada");
        _context.Ferias.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
