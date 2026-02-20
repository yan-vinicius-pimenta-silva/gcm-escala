using EscalaGcm.Application.DTOs.Escalas;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Entities;
using EscalaGcm.Domain.Enums;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class EscalaService : IEscalaService
{
    private readonly AppDbContext _context;
    private readonly IConflictValidationService _conflictService;
    private readonly ISectorRuleService _sectorRuleService;

    public EscalaService(AppDbContext context, IConflictValidationService conflictService, ISectorRuleService sectorRuleService)
    {
        _context = context;
        _conflictService = conflictService;
        _sectorRuleService = sectorRuleService;
    }

    public async Task<List<EscalaDto>> GetAllAsync(int? ano, int? mes, int? quinzena, int? setorId)
    {
        var query = _context.Escalas.Include(e => e.Setor).AsQueryable();
        if (ano.HasValue) query = query.Where(e => e.Ano == ano.Value);
        if (mes.HasValue) query = query.Where(e => e.Mes == mes.Value);
        if (quinzena.HasValue) query = query.Where(e => e.Quinzena == quinzena.Value);
        if (setorId.HasValue) query = query.Where(e => e.SetorId == setorId.Value);

        return await query.OrderByDescending(e => e.Ano).ThenByDescending(e => e.Mes).ThenBy(e => e.Quinzena)
            .Select(e => new EscalaDto(e.Id, e.Ano, e.Mes, e.Quinzena, e.SetorId, e.Setor.Nome, e.Status, new List<EscalaItemDto>()))
            .ToListAsync();
    }

    public async Task<EscalaDto?> GetByIdAsync(int id)
    {
        var e = await _context.Escalas
            .Include(e => e.Setor)
            .Include(e => e.Itens).ThenInclude(i => i.Turno)
            .Include(e => e.Itens).ThenInclude(i => i.Horario)
            .Include(e => e.Itens).ThenInclude(i => i.Alocacoes).ThenInclude(a => a.Guarda)
            .Include(e => e.Itens).ThenInclude(i => i.Alocacoes).ThenInclude(a => a.Equipe).ThenInclude(eq => eq!.Membros).ThenInclude(m => m.Guarda)
            .Include(e => e.Itens).ThenInclude(i => i.Alocacoes).ThenInclude(a => a.Viatura)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (e == null) return null;

        return new EscalaDto(e.Id, e.Ano, e.Mes, e.Quinzena, e.SetorId, e.Setor.Nome, e.Status,
            e.Itens.OrderBy(i => i.Data).ThenBy(i => i.Turno.Nome).Select(i => MapItem(i)).ToList());
    }

    public async Task<(EscalaDto? Result, string? Error)> CreateAsync(CreateEscalaRequest request)
    {
        var exists = await _context.Escalas.AnyAsync(e =>
            e.Ano == request.Ano && e.Mes == request.Mes && e.Quinzena == request.Quinzena && e.SetorId == request.SetorId);
        if (exists) return (null, "Já existe uma escala para este setor, mês, ano e quinzena");

        var entity = new Escala { Ano = request.Ano, Mes = request.Mes, Quinzena = request.Quinzena, SetorId = request.SetorId, Status = StatusEscala.Rascunho };
        _context.Escalas.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id), null);
    }

    public async Task<(EscalaItemDto? Result, List<ConflictError>? Errors)> AddItemAsync(int escalaId, AddEscalaItemRequest request)
    {
        var escala = await _context.Escalas.Include(e => e.Setor).FirstOrDefaultAsync(e => e.Id == escalaId);
        if (escala == null) return (null, [new ConflictError("ERRO", "Escala não encontrada")]);
        if (escala.Status != StatusEscala.Rascunho) return (null, [new ConflictError("ERRO", "Escala não está em rascunho")]);

        // REVIEW: DateOnly.Parse throws FormatException on bad input -> 500. Use TryParse and return a validation error.
        var data = DateOnly.Parse(request.Data);

        // Validate date belongs to the fortnight
        var dateError = ValidateFortnightDate(data, escala.Ano, escala.Mes, escala.Quinzena);
        if (dateError != null) return (null, [new ConflictError("ERRO", dateError)]);

        // Validate sector rules
        var sectorErrors = _sectorRuleService.ValidateSectorRules(escala.Setor.Tipo, request.Alocacoes);

        // Validate conflicts
        var conflictErrors = await _conflictService.ValidateAllocationsAsync(data, request.HorarioId, request.Alocacoes, null, request.Regime);

        var allErrors = sectorErrors.Concat(conflictErrors).ToList();
        if (allErrors.Count > 0) return (null, allErrors);

        // REVIEW: Two SaveChangesAsync calls for one logical operation. If the second fails,
        // an orphaned EscalaItem remains. Use a single SaveChangesAsync or wrap in a transaction.
        var item = new EscalaItem { EscalaId = escalaId, Data = data, TurnoId = request.TurnoId, HorarioId = request.HorarioId, Regime = request.Regime, Observacao = request.Observacao };
        _context.EscalaItens.Add(item);
        await _context.SaveChangesAsync();

        foreach (var aloc in request.Alocacoes)
        {
            _context.EscalaAlocacoes.Add(new EscalaAlocacao
            {
                EscalaItemId = item.Id, GuardaId = aloc.GuardaId, EquipeId = aloc.EquipeId,
                Funcao = aloc.Funcao, ViaturaId = aloc.ViaturaId
            });
        }
        await _context.SaveChangesAsync();

        var loaded = await _context.EscalaItens
            .Include(i => i.Turno).Include(i => i.Horario)
            .Include(i => i.Alocacoes).ThenInclude(a => a.Guarda)
            .Include(i => i.Alocacoes).ThenInclude(a => a.Equipe).ThenInclude(eq => eq!.Membros).ThenInclude(m => m.Guarda)
            .Include(i => i.Alocacoes).ThenInclude(a => a.Viatura)
            .FirstAsync(i => i.Id == item.Id);

        return (MapItem(loaded), null);
    }

    public async Task<(EscalaItemDto? Result, List<ConflictError>? Errors)> UpdateItemAsync(int escalaId, int itemId, UpdateEscalaItemRequest request)
    {
        var escala = await _context.Escalas.Include(e => e.Setor).FirstOrDefaultAsync(e => e.Id == escalaId);
        if (escala == null) return (null, [new ConflictError("ERRO", "Escala não encontrada")]);
        if (escala.Status != StatusEscala.Rascunho) return (null, [new ConflictError("ERRO", "Escala não está em rascunho")]);

        var item = await _context.EscalaItens.Include(i => i.Alocacoes).FirstOrDefaultAsync(i => i.Id == itemId && i.EscalaId == escalaId);
        if (item == null) return (null, [new ConflictError("ERRO", "Item não encontrado")]);

        // REVIEW: Same DateOnly.Parse issue as in AddItemAsync -- use TryParse.
        var data = DateOnly.Parse(request.Data);
        var dateError = ValidateFortnightDate(data, escala.Ano, escala.Mes, escala.Quinzena);
        if (dateError != null) return (null, [new ConflictError("ERRO", dateError)]);

        var sectorErrors = _sectorRuleService.ValidateSectorRules(escala.Setor.Tipo, request.Alocacoes);
        var conflictErrors = await _conflictService.ValidateAllocationsAsync(data, request.HorarioId, request.Alocacoes, itemId, request.Regime);
        var allErrors = sectorErrors.Concat(conflictErrors).ToList();
        if (allErrors.Count > 0) return (null, allErrors);

        item.Data = data;
        item.TurnoId = request.TurnoId;
        item.HorarioId = request.HorarioId;
        item.Regime = request.Regime;
        item.Observacao = request.Observacao;

        _context.EscalaAlocacoes.RemoveRange(item.Alocacoes);
        foreach (var aloc in request.Alocacoes)
        {
            _context.EscalaAlocacoes.Add(new EscalaAlocacao
            {
                EscalaItemId = item.Id, GuardaId = aloc.GuardaId, EquipeId = aloc.EquipeId,
                Funcao = aloc.Funcao, ViaturaId = aloc.ViaturaId
            });
        }
        await _context.SaveChangesAsync();

        var loaded = await _context.EscalaItens
            .Include(i => i.Turno).Include(i => i.Horario)
            .Include(i => i.Alocacoes).ThenInclude(a => a.Guarda)
            .Include(i => i.Alocacoes).ThenInclude(a => a.Equipe).ThenInclude(eq => eq!.Membros).ThenInclude(m => m.Guarda)
            .Include(i => i.Alocacoes).ThenInclude(a => a.Viatura)
            .FirstAsync(i => i.Id == item.Id);

        return (MapItem(loaded), null);
    }

    public async Task<(bool Success, string? Error)> DeleteItemAsync(int escalaId, int itemId)
    {
        var escala = await _context.Escalas.FindAsync(escalaId);
        if (escala == null) return (false, "Escala não encontrada");
        if (escala.Status != StatusEscala.Rascunho) return (false, "Escala não está em rascunho");

        var item = await _context.EscalaItens.Include(i => i.Alocacoes).FirstOrDefaultAsync(i => i.Id == itemId && i.EscalaId == escalaId);
        if (item == null) return (false, "Item não encontrado");

        _context.EscalaAlocacoes.RemoveRange(item.Alocacoes);
        _context.EscalaItens.Remove(item);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> PublicarAsync(int id)
    {
        var escala = await _context.Escalas.FindAsync(id);
        if (escala == null) return (false, "Escala não encontrada");
        if (escala.Status != StatusEscala.Rascunho) return (false, "Somente escalas em rascunho podem ser publicadas");
        escala.Status = StatusEscala.Publicada;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    // REVIEW: Manual cascade delete. Consider configuring cascade delete in EF model config
    // (OnDelete(DeleteBehavior.Cascade)) so the DB handles this atomically.
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var escala = await _context.Escalas.Include(e => e.Itens).ThenInclude(i => i.Alocacoes).FirstOrDefaultAsync(e => e.Id == id);
        if (escala == null) return (false, "Escala não encontrada");

        foreach (var item in escala.Itens)
            _context.EscalaAlocacoes.RemoveRange(item.Alocacoes);
        _context.EscalaItens.RemoveRange(escala.Itens);
        _context.Escalas.Remove(escala);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private static string? ValidateFortnightDate(DateOnly data, int ano, int mes, int quinzena)
    {
        if (data.Year != ano || data.Month != mes)
            return $"A data deve pertencer a {mes:D2}/{ano}";
        if (quinzena == 1 && data.Day > 15)
            return "Para a 1ª quinzena, o dia deve ser entre 1 e 15";
        if (quinzena == 2 && data.Day < 16)
            return "Para a 2ª quinzena, o dia deve ser 16 ou posterior";
        return null;
    }

    private static EscalaItemDto MapItem(EscalaItem i) => new(
        i.Id, i.EscalaId, i.Data.ToString("yyyy-MM-dd"),
        i.TurnoId, i.Turno.Nome, i.HorarioId, i.Horario.Descricao, i.Regime, i.Observacao,
        i.Alocacoes.Select(a => new EscalaAlocacaoDto(
            a.Id, a.GuardaId, a.Guarda?.Nome, a.EquipeId, FormatEquipeNomeComIntegrantes(a.Equipe),
            a.Funcao, a.ViaturaId, a.Viatura?.Identificador)).ToList());

    private static string? FormatEquipeNomeComIntegrantes(Equipe? equipe)
    {
        if (equipe == null) return null;

        var integrantes = equipe.Membros
            .Select(m => m.Guarda?.Nome)
            .Where(nome => !string.IsNullOrWhiteSpace(nome))
            .Cast<string>()
            .ToList();

        if (!integrantes.Any()) return equipe.Nome;

        return $"{equipe.Nome} [{string.Join(", ", integrantes)}]";
    }
}
