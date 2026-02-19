using EscalaGcm.Domain.Entities;
using EscalaGcm.Domain.Enums;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Tests.Helpers;

/// <summary>
/// Helpers para criar AppDbContext em memória e popular entidades de teste.
/// Cada teste deve chamar CreateContext() para obter um banco isolado (Guid único).
/// </summary>
public static class DbHelper
{
    public static AppDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    // ── Entidades simples ─────────────────────────────────────────────────────

    public static async Task<Guarda> AddGuardaAsync(AppDbContext ctx, string nome = "Guarda Teste")
    {
        var g = new Guarda { Nome = nome, Ativo = true };
        ctx.Guardas.Add(g);
        await ctx.SaveChangesAsync();
        return g;
    }

    public static async Task<Horario> AddHorarioAsync(AppDbContext ctx, TimeOnly inicio, TimeOnly fim, string? desc = null)
    {
        var h = new Horario { Inicio = inicio, Fim = fim, Descricao = desc ?? $"{inicio}-{fim}", Ativo = true };
        ctx.Horarios.Add(h);
        await ctx.SaveChangesAsync();
        return h;
    }

    public static async Task<Evento> AddEventoAsync(AppDbContext ctx, string nome, DateOnly dataInicio, DateOnly dataFim)
    {
        var e = new Evento { Nome = nome, DataInicio = dataInicio, DataFim = dataFim };
        ctx.Eventos.Add(e);
        await ctx.SaveChangesAsync();
        return e;
    }

    // ── Escala + Item + Alocação ──────────────────────────────────────────────

    /// <summary>
    /// Cria a cadeia mínima Escala → EscalaItem → EscalaAlocacao para que
    /// RetService.ValidateAsync encontre uma alocação do guarda na data e horário indicados.
    /// Cada chamada cria Setor e Turno novos (isolados por teste).
    /// </summary>
    public static async Task<EscalaAlocacao> AddAlocacaoAsync(
        AppDbContext ctx,
        int guardaId,
        DateOnly data,
        Horario horario)
    {
        // Setor mínimo
        var setor = new Setor { Nome = $"Setor-{Guid.NewGuid():N}", Tipo = TipoSetor.Padrao, Ativo = true };
        ctx.Setores.Add(setor);
        await ctx.SaveChangesAsync();

        // Turno mínimo
        var turno = new Turno { Nome = "Turno Teste", Ativo = true };
        ctx.Turnos.Add(turno);
        await ctx.SaveChangesAsync();

        // Escala
        var escala = new Escala
        {
            Ano = data.Year,
            Mes = data.Month,
            Quinzena = data.Day <= 15 ? 1 : 2,
            SetorId = setor.Id,
            Status = StatusEscala.Publicada
        };
        ctx.Escalas.Add(escala);
        await ctx.SaveChangesAsync();

        // Item
        var item = new EscalaItem
        {
            EscalaId = escala.Id,
            Data = data,
            TurnoId = turno.Id,
            HorarioId = horario.Id
        };
        ctx.EscalaItens.Add(item);
        await ctx.SaveChangesAsync();

        // Alocação
        var alocacao = new EscalaAlocacao
        {
            EscalaItemId = item.Id,
            GuardaId = guardaId,
            Funcao = FuncaoAlocacao.Integrante
        };
        ctx.EscalaAlocacoes.Add(alocacao);
        await ctx.SaveChangesAsync();

        return alocacao;
    }
}
