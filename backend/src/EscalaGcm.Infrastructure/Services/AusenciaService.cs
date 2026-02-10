using EscalaGcm.Application.DTOs.Ausencias;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class AusenciaService : IAusenciaService
{
    private readonly AppDbContext _context;
    public AusenciaService(AppDbContext context) => _context = context;

    public async Task<List<AusenciaDto>> GetAllAsync() =>
        await _context.Ausencias.Include(a => a.Guarda).OrderByDescending(a => a.DataInicio)
            .Select(a => new AusenciaDto(a.Id, a.GuardaId, a.Guarda.Nome,
                a.DataInicio.ToString("yyyy-MM-dd"), a.DataFim.ToString("yyyy-MM-dd"), a.Motivo, a.Observacoes))
            .ToListAsync();

    public async Task<AusenciaDto?> GetByIdAsync(int id)
    {
        var a = await _context.Ausencias.Include(a => a.Guarda).FirstOrDefaultAsync(a => a.Id == id);
        if (a == null) return null;
        return new AusenciaDto(a.Id, a.GuardaId, a.Guarda.Nome,
            a.DataInicio.ToString("yyyy-MM-dd"), a.DataFim.ToString("yyyy-MM-dd"), a.Motivo, a.Observacoes);
    }

    public async Task<(AusenciaDto? Result, string? Error)> CreateAsync(CreateAusenciaRequest request)
    {
        var inicio = DateOnly.Parse(request.DataInicio);
        var fim = DateOnly.Parse(request.DataFim);
        if (fim < inicio) return (null, "Data fim deve ser maior ou igual à data início");

        var overlap = await _context.Ausencias.AnyAsync(a =>
            a.GuardaId == request.GuardaId && a.DataInicio <= fim && a.DataFim >= inicio);
        if (overlap) return (null, "Já existe ausência cadastrada neste período para este guarda");

        var entity = new Ausencia { GuardaId = request.GuardaId, DataInicio = inicio, DataFim = fim, Motivo = request.Motivo, Observacoes = request.Observacoes };
        _context.Ausencias.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(AusenciaDto? Result, string? Error)> UpdateAsync(int id, UpdateAusenciaRequest request)
    {
        var entity = await _context.Ausencias.FindAsync(id);
        if (entity == null) return (null, "Ausência não encontrada");

        var inicio = DateOnly.Parse(request.DataInicio);
        var fim = DateOnly.Parse(request.DataFim);
        if (fim < inicio) return (null, "Data fim deve ser maior ou igual à data início");

        var overlap = await _context.Ausencias.AnyAsync(a =>
            a.GuardaId == request.GuardaId && a.Id != id && a.DataInicio <= fim && a.DataFim >= inicio);
        if (overlap) return (null, "Já existe ausência cadastrada neste período para este guarda");

        entity.GuardaId = request.GuardaId;
        entity.DataInicio = inicio;
        entity.DataFim = fim;
        entity.Motivo = request.Motivo;
        entity.Observacoes = request.Observacoes;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var entity = await _context.Ausencias.FindAsync(id);
        if (entity == null) return (false, "Ausência não encontrada");
        _context.Ausencias.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
