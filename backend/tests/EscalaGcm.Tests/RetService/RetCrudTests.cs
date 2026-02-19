using EscalaGcm.Application.DTOs.Rets;
using EscalaGcm.Domain.Enums;
using EscalaGcm.Infrastructure.Services;
using EscalaGcm.Tests.Helpers;
using Xunit;

namespace EscalaGcm.Tests.RetService;

/// <summary>
/// Testa as operações CRUD de RetService:
/// GetAll (com e sem filtros), GetById, Create, Update, Delete.
/// </summary>
public class RetCrudTests
{
    // ── GetAll ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_SemFiltros_RetornaTodosOsRets()
    {
        await using var ctx = DbHelper.CreateContext();
        var g1 = await DbHelper.AddGuardaAsync(ctx, "Guarda A");
        var g2 = await DbHelper.AddGuardaAsync(ctx, "Guarda B");
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        await svc.CreateAsync(new CreateRetRequest(g1.Id, "2026-02-05", "08:00", TipoRet.Mensal, null, null));
        await svc.CreateAsync(new CreateRetRequest(g2.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        var lista = await svc.GetAllAsync();

        Assert.Equal(2, lista.Count);
    }

    [Fact]
    public async Task GetAll_FiltrandoPorGuarda_RetornaApenasDoGuarda()
    {
        await using var ctx = DbHelper.CreateContext();
        var g1 = await DbHelper.AddGuardaAsync(ctx, "Guarda A");
        var g2 = await DbHelper.AddGuardaAsync(ctx, "Guarda B");
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        await svc.CreateAsync(new CreateRetRequest(g1.Id, "2026-02-05", "08:00", TipoRet.Mensal, null, null));
        await svc.CreateAsync(new CreateRetRequest(g2.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        var lista = await svc.GetAllAsync(guardaId: g1.Id);

        Assert.Single(lista);
        Assert.Equal(g1.Id, lista[0].GuardaId);
    }

    [Fact]
    public async Task GetAll_FiltrandoPorMes_RetornaApenasDoMes()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        await svc.CreateAsync(new CreateRetRequest(guarda.Id, "2026-02-05", "08:00", TipoRet.Mensal, null, null));
        await svc.CreateAsync(new CreateRetRequest(guarda.Id, "2026-03-10", "08:00", TipoRet.Mensal, null, null));

        var lista = await svc.GetAllAsync(mes: 2, ano: 2026);

        Assert.Single(lista);
        Assert.Equal("2026-02-05", lista[0].Data);
    }

    [Fact]
    public async Task GetAll_FiltrandoPorAno_RetornaApenasDoAno()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        await svc.CreateAsync(new CreateRetRequest(guarda.Id, "2026-02-05", "08:00", TipoRet.Mensal, null, null));
        await svc.CreateAsync(new CreateRetRequest(guarda.Id, "2027-02-05", "08:00", TipoRet.Mensal, null, null));

        var lista = await svc.GetAllAsync(ano: 2026);

        Assert.Single(lista);
    }

    [Fact]
    public async Task GetAll_BancoVazio_RetornaListaVazia()
    {
        await using var ctx = DbHelper.CreateContext();
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var lista = await svc.GetAllAsync();

        Assert.Empty(lista);
    }

    // ── GetById ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_RetExistente_RetornaDto()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx, "Anderson Silva");
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (created, _) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, "obs teste"));
        Assert.NotNull(created);

        var ret = await svc.GetByIdAsync(created.Id);

        Assert.NotNull(ret);
        Assert.Equal(guarda.Id, ret.GuardaId);
        Assert.Equal("Anderson Silva", ret.GuardaNome);
        Assert.Equal("2026-02-10", ret.Data);
        Assert.Equal("08:00", ret.HorarioInicio);
        Assert.Equal("16:00", ret.HorarioFim);
        Assert.Equal(TipoRet.Mensal, ret.Tipo);
        Assert.Equal("obs teste", ret.Observacao);
    }

    [Fact]
    public async Task GetById_IdInexistente_RetornaNull()
    {
        await using var ctx = DbHelper.CreateContext();
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var ret = await svc.GetByIdAsync(9999);

        Assert.Null(ret);
    }

    // ── Create ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_DadosValidos_RetornaRetCriado()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-15", "14:00", TipoRet.Mensal, null, "criado ok"));

        Assert.Null(error);
        Assert.NotNull(result);
        Assert.Equal("2026-02-15", result.Data);
        Assert.Equal("14:00", result.HorarioInicio);
        Assert.Equal("22:00", result.HorarioFim); // 8h depois de 14:00
        Assert.Equal("criado ok", result.Observacao);
    }

    [Fact]
    public async Task Create_HorarioFimCalculadoAutomaticamente_8hAposInicio()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, _) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "10:00", TipoRet.Mensal, null, null));

        Assert.NotNull(result);
        Assert.Equal("18:00", result.HorarioFim);
    }

    // ── Update ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_IdInexistente_RetornaErro()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.UpdateAsync(
            9999, new UpdateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(result);
        Assert.Equal("RET não encontrado", error);
    }

    [Fact]
    public async Task Update_AlteraObservacao_Sucesso()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (created, _) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, "original"));
        Assert.NotNull(created);

        var (updated, err) = await svc.UpdateAsync(
            created.Id, new UpdateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, "atualizado"));

        Assert.Null(err);
        Assert.NotNull(updated);
        Assert.Equal("atualizado", updated.Observacao);
    }

    // ── Delete ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_IdExistente_RemoveERetornaSuccess()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (created, _) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));
        Assert.NotNull(created);

        var (success, err) = await svc.DeleteAsync(created.Id);

        Assert.True(success);
        Assert.Null(err);
        Assert.Null(await svc.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task Delete_IdInexistente_RetornaErro()
    {
        await using var ctx = DbHelper.CreateContext();
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (success, err) = await svc.DeleteAsync(9999);

        Assert.False(success);
        Assert.Equal("RET não encontrado", err);
    }

    // ── Dados retornados ──────────────────────────────────────────────────

    [Fact]
    public async Task Create_ComEvento_RetornaEventoNome()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var evento = await DbHelper.AddEventoAsync(ctx, "Carnaval 2026",
            new DateOnly(2026, 2, 14), new DateOnly(2026, 2, 17));
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, _) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-19", "08:00", TipoRet.Evento, evento.Id, null));

        Assert.NotNull(result);
        Assert.Equal(evento.Id, result.EventoId);
        Assert.Equal("Carnaval 2026", result.EventoNome);
    }
}
