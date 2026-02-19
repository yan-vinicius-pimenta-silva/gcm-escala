using EscalaGcm.Application.DTOs.Rets;
using EscalaGcm.Domain.Enums;
using EscalaGcm.Infrastructure.Services;
using EscalaGcm.Tests.Helpers;
using Xunit;

namespace EscalaGcm.Tests.RetService;

/// <summary>
/// Cenários de ponta a ponta extraídos diretamente do documento RET.md.
/// Cada teste reproduz um cenário VÁLIDO ou INVÁLIDO do spec.
/// </summary>
public class RetE2eTests
{
    // ═══════════════════════════════════════════════════════════════════════
    // CENÁRIO VÁLIDO 1 — RET Mensal sem escalas próximas
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Guarda sem nenhuma escala no mês. RET Mensal deve ser permitido.
    /// </summary>
    [Fact]
    public async Task CenarioValido1_RetMensal_SemEscalasNaBD_Permitido()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx, "Anderson Silva");
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
        Assert.Equal(TipoRet.Mensal, result.Tipo);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CENÁRIO VÁLIDO 2 — RET sanduíche com descansos exatos
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Escala antes termina às 19:00 de 10/02.
    /// RET em 11/02 07:00-15:00 (exatamente 12h após escala antes).
    /// Escala depois começa às 03:00 de 12/02 (exatamente 12h após RET, total=32h).
    /// → PERMITIDO
    /// </summary>
    [Fact]
    public async Task CenarioValido2_RetSanduiche_Descansos12h_Total32h_Permitido()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx, "Bruno Santos");
        var hAntes = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(7, 0), new TimeOnly(19, 0), "07-19 Diurno");
        var hDepois = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(3, 0), new TimeOnly(11, 0), "03-11");

        // Escala diurna no dia 10/02 → termina 19:00
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 10), hAntes);
        // Próxima escala começa 12/02 03:00 → 12h após RET fim de 15:00 em 11/02
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 12), hDepois);

        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        // RET começa 11/02 07:00 → 12h após 19:00 de 10/02 ✓
        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-11", "07:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CENÁRIO VÁLIDO 3 — RET EVENTO com data posterior ao evento
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Carnaval: 14-17/02/2026.
    /// RET EVENTO em 19/02/2026 (data DEPOIS do evento encerrar).
    /// O spec diz: "O evento deve ocorrer no mesmo mês do RET" — não que o RET deve estar dentro do evento.
    /// → PERMITIDO (CENÁRIO VÁLIDO 3 do RET.md)
    /// </summary>
    [Fact]
    public async Task CenarioValido3_RetEvento_DataAposEventoNoMesmoMes_Permitido()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx, "Carlos Oliveira");
        var carnaval = await DbHelper.AddEventoAsync(ctx, "Carnaval",
            new DateOnly(2026, 2, 14), new DateOnly(2026, 2, 17));
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-19", "08:00", TipoRet.Evento, carnaval.Id, null));

        Assert.Null(error);
        Assert.NotNull(result);
        Assert.Equal(TipoRet.Evento, result.Tipo);
        Assert.Equal(carnaval.Id, result.EventoId);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CENÁRIO INVÁLIDO 1 — Descanso insuficiente antes
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Escala diurna termina 19:00 de 10/02.
    /// RET em 11/02 06:00 → somente 11h de descanso antes (< 12h).
    /// → BLOQUEADO
    /// </summary>
    [Fact]
    public async Task CenarioInvalido1_DescansoAntesInsuficiente_Bloqueado()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx, "Daniel Pereira");
        var hDiurno = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(7, 0), new TimeOnly(19, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 10), hDiurno);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-11", "06:00", TipoRet.Mensal, null, null));

        Assert.Null(result);
        Assert.NotNull(error);
        Assert.Contains("Descanso insuficiente antes do RET", error);
        Assert.Contains("11,0h", error.Replace(".", ","));
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CENÁRIO INVÁLIDO 2 — RET EVENTO sem evento no mês
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// RET EVENTO em fevereiro mas o único evento cadastrado é em março.
    /// → BLOQUEADO
    /// </summary>
    [Fact]
    public async Task CenarioInvalido2_RetEvento_SemEventoNoMes_Bloqueado()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx, "Eduardo Costa");
        var eventoMarco = await DbHelper.AddEventoAsync(ctx, "Evento Marco",
            new DateOnly(2026, 3, 10), new DateOnly(2026, 3, 12));
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        // Tenta criar RET Evento em fevereiro com um evento que ocorre em março
        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-15", "08:00", TipoRet.Evento, eventoMarco.Id, null));

        Assert.Null(result);
        Assert.NotNull(error);
        Assert.Contains("não ocorre no mês do RET", error);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CENÁRIO INVÁLIDO 3 — Segundo RET Mensal no mesmo mês
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Guarda já tem 1 RET Mensal em fevereiro.
    /// Tentativa de criar um segundo → BLOQUEADO.
    /// </summary>
    [Fact]
    public async Task CenarioInvalido3_SegundoRetMensalNoMes_Bloqueado()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx, "Fernando Almeida");
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        // Primeiro RET — deve passar
        var (r1, e1) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-05", "08:00", TipoRet.Mensal, null, null));
        Assert.Null(e1);
        Assert.NotNull(r1);

        // Segundo RET no mesmo mês — deve bloquear
        var (r2, e2) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-20", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(r2);
        Assert.Contains("já possui 1 RET Mensal neste mês", e2);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CENÁRIO EXTRA — Escala noturna cruzando meia-noite + RET
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Escala noturna 12x36: 09/02 19:00 → 10/02 07:00.
    /// RET em 10/02 às 08:00: somente 1h de descanso → BLOQUEADO.
    /// Valida que o cálculo de fim de turno noturno está correto (não repete bug do "fim = mesmo dia").
    /// </summary>
    [Fact]
    public async Task CenarioExtra_EscalaNoturnaMeiaNoite_DescansoInsuficiente_Bloqueado()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx, "Gustavo Lima");
        var hNoturno = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(19, 0), new TimeOnly(7, 0), "19-07 Noturno");
        // Escala noturna no dia 09/02; fim real = 10/02 07:00
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 9), hNoturno);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        // RET em 10/02 às 08:00 → apenas 1h após fim da escala às 07:00 do dia 10/02
        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(result);
        Assert.Contains("Descanso insuficiente antes do RET", error);
    }
}
