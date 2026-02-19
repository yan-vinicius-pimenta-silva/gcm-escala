using EscalaGcm.Application.DTOs.Rets;
using EscalaGcm.Domain.Enums;
using EscalaGcm.Infrastructure.Services;
using EscalaGcm.Tests.Helpers;
using Xunit;

namespace EscalaGcm.Tests.RetService;

/// <summary>
/// Testa todas as regras de validação de RetService:
///   1. Tipo Evento requer EventoId e evento deve ocorrer no mesmo mês
///   2. Máx 1 RET Mensal por mês por guarda
///   3. Máx 1 RET Evento por mês por guarda
///   4. Descanso mínimo de 12h antes do RET
///   5. Descanso mínimo de 12h após o RET
///   6. Intervalo total ≥ 32h quando há escala antes E depois
///   7. Turnos noturnos (cruzando meia-noite) calculados corretamente
/// </summary>
public class RetValidationTests
{
    // ═══════════════════════════════════════════════════════════════════════
    // 1. Tipo Evento — requer EventoId
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateRet_Evento_SemEventoId_RetornaErro()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var req = new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Evento, null, null);
        var (result, error) = await svc.CreateAsync(req);

        Assert.Null(result);
        Assert.Contains("requer um evento vinculado", error);
    }

    [Fact]
    public async Task CreateRet_Evento_EventoInexistente_RetornaErro()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var req = new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Evento, 9999, null);
        var (result, error) = await svc.CreateAsync(req);

        Assert.Null(result);
        Assert.Contains("Evento não encontrado", error);
    }

    [Fact]
    public async Task CreateRet_Evento_EventoForaDoMes_RetornaErro()
    {
        // Evento ocorre em janeiro; RET é em fevereiro → deve bloquear
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var evento = await DbHelper.AddEventoAsync(ctx, "Evento Jan",
            new DateOnly(2026, 1, 10), new DateOnly(2026, 1, 15));
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var req = new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Evento, evento.Id, null);
        var (result, error) = await svc.CreateAsync(req);

        Assert.Null(result);
        Assert.Contains("não ocorre no mês do RET", error);
    }

    [Fact]
    public async Task CreateRet_Evento_EventoNoMesDoRet_DataRetAposEvento_Sucesso()
    {
        // CENÁRIO VÁLIDO 3 do spec: RET EVENTO em 19/02 com Carnaval 14-17/02 → deve permitir
        // O RET pode ser depois do evento; basta o evento ocorrer no mesmo mês.
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var carnaval = await DbHelper.AddEventoAsync(ctx, "Carnaval",
            new DateOnly(2026, 2, 14), new DateOnly(2026, 2, 17));
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var req = new CreateRetRequest(guarda.Id, "2026-02-19", "08:00", TipoRet.Evento, carnaval.Id, null);
        var (result, error) = await svc.CreateAsync(req);

        Assert.Null(error);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateRet_Evento_EventoSobrepoeMesDoRet_Sucesso()
    {
        // Evento começa em janeiro e termina em fevereiro: deve ser permitido para RET em fevereiro
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var evento = await DbHelper.AddEventoAsync(ctx, "Evento Jan-Fev",
            new DateOnly(2026, 1, 28), new DateOnly(2026, 2, 5));
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var req = new CreateRetRequest(guarda.Id, "2026-02-03", "08:00", TipoRet.Evento, evento.Id, null);
        var (result, error) = await svc.CreateAsync(req);

        Assert.Null(error);
        Assert.NotNull(result);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 2. Máx 1 RET Mensal por mês
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateRet_Mensal_PrimeiroNoMes_Sucesso()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var req = new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null);
        var (result, error) = await svc.CreateAsync(req);

        Assert.Null(error);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateRet_Mensal_SegundoNoMesmoMes_RetornaErro()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        // Primeiro RET Mensal
        await svc.CreateAsync(new CreateRetRequest(guarda.Id, "2026-02-05", "08:00", TipoRet.Mensal, null, null));

        // Segundo no mesmo mês → deve bloquear
        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-20", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(result);
        Assert.Contains("já possui 1 RET Mensal neste mês", error);
    }

    [Fact]
    public async Task CreateRet_Mensal_SegundoEmMesDiferente_Sucesso()
    {
        // Cada mês tem sua cota independente
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        await svc.CreateAsync(new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));
        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-03-10", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 3. Máx 1 RET Evento por mês
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateRet_Evento_SegundoNoMesmoMes_RetornaErro()
    {
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var evento = await DbHelper.AddEventoAsync(ctx, "Carnaval",
            new DateOnly(2026, 2, 14), new DateOnly(2026, 2, 17));
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        // Primeiro RET Evento
        await svc.CreateAsync(new CreateRetRequest(guarda.Id, "2026-02-05", "08:00", TipoRet.Evento, evento.Id, null));

        // Segundo no mesmo mês → deve bloquear
        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-19", "08:00", TipoRet.Evento, evento.Id, null));

        Assert.Null(result);
        Assert.Contains("já possui 1 RET de Evento neste mês", error);
    }

    [Fact]
    public async Task CreateRet_MensalEEventoMesmoMes_SaoCotasSeparadas()
    {
        // 1 Mensal + 1 Evento no mesmo mês é permitido (cotas independentes)
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var evento = await DbHelper.AddEventoAsync(ctx, "Carnaval",
            new DateOnly(2026, 2, 14), new DateOnly(2026, 2, 17));
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (r1, e1) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-05", "08:00", TipoRet.Mensal, null, null));
        var (r2, e2) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-19", "08:00", TipoRet.Evento, evento.Id, null));

        Assert.Null(e1);
        Assert.Null(e2);
        Assert.NotNull(r1);
        Assert.NotNull(r2);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 4. Descanso 12h antes do RET
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateRet_DescansoAntes_Exatamente12h_Sucesso()
    {
        // Escala diurna termina às 19:00 do dia 10/02
        // RET começa às 07:00 do dia 11/02 → exatamente 12h → PERMITIDO
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hDiurno = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(7, 0), new TimeOnly(19, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 10), hDiurno);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-11", "07:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateRet_DescansoAntes_Menor12h_RetornaErro()
    {
        // Escala diurna termina às 19:00 do dia 10/02
        // RET começa às 06:00 do dia 11/02 → somente 11h → BLOQUEADO
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hDiurno = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(7, 0), new TimeOnly(19, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 10), hDiurno);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-11", "06:00", TipoRet.Mensal, null, null));

        Assert.Null(result);
        Assert.Contains("Descanso insuficiente antes do RET", error);
    }

    [Fact]
    public async Task CreateRet_SemEscalaAntes_SemRestricaoDescanso()
    {
        // Sem escalas anteriores, regra de descanso não se aplica
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 5. Descanso 12h após o RET
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateRet_DescansoApos_Exatamente12h_Sucesso()
    {
        // RET começa 08:00 dia 10/02 → termina 16:00 dia 10/02
        // Escala começa às 04:00 do dia 11/02 → exatamente 12h após 16:00 → PERMITIDO
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hProximo = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(4, 0), new TimeOnly(12, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 11), hProximo);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateRet_DescansoApos_Menor12h_RetornaErro()
    {
        // RET começa 08:00 dia 10/02 → termina 16:00 dia 10/02
        // Escala começa às 03:00 do dia 11/02 → somente 11h → BLOQUEADO
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hProximo = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(3, 0), new TimeOnly(11, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 11), hProximo);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(result);
        Assert.Contains("Descanso insuficiente após o RET", error);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 6. Intervalo total ≥ 32h (escala antes E depois)
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateRet_IntervaloTotal_Exatamente32h_Sucesso()
    {
        // Escala antes termina 19:00 dia 10/02
        // RET: 11/02 07:00-15:00 (12h após escala anterior)
        // Escala depois começa 12/02 03:00 (12h após fim do RET)
        // Total: 12/02 03:00 - 10/02 19:00 = 32h → PERMITIDO
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hAntes = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(7, 0), new TimeOnly(19, 0));
        var hDepois = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(3, 0), new TimeOnly(11, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 10), hAntes);
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 12), hDepois);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-11", "07:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 7. Turnos noturnos cruzando meia-noite
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateRet_EscalaNoturnaCruzaMeiaNoite_FimCalculadoDiaSeguinte()
    {
        // Escala noturna: data=09/02, horario 19:00-07:00 (fim real = 10/02 07:00)
        // RET começa 10/02 08:00 → somente 1h após fim real → BLOQUEADO
        // Sem a correção de meia-noite, o fim seria calculado como 09/02 07:00 (errado → 25h → passaria)
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hNoturno = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(19, 0), new TimeOnly(7, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 9), hNoturno);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, null));

        Assert.Null(result);
        Assert.Contains("Descanso insuficiente antes do RET", error);
    }

    [Fact]
    public async Task CreateRet_EscalaNoturnaCruzaMeiaNoite_Descanso12h_Sucesso()
    {
        // Escala noturna: data=09/02, horario 19:00-07:00 (fim real = 10/02 07:00)
        // RET começa 10/02 19:00 → exatamente 12h após fim real → PERMITIDO
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hNoturno = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(19, 0), new TimeOnly(7, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 9), hNoturno);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "19:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateRet_RetNoturnoCruzaMeiaNoite_FimCalculadoDiaSeguinte()
    {
        // RET: data=10/02, inicio=22:00, fim=06:00 (retFim real = 11/02 06:00)
        // Próxima escala: 11/02 07:00 → somente 1h após retFim → BLOQUEADO
        // Sem a correção, retFim seria 10/02 06:00 (errado → 25h → passaria)
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hProxEscala = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(7, 0), new TimeOnly(15, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 11), hProxEscala);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        // RET noturno: inicio=22:00, fim=06:00 (overnight)
        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "22:00", TipoRet.Mensal, null, null));

        Assert.Null(result);
        Assert.Contains("Descanso insuficiente após o RET", error);
    }

    [Fact]
    public async Task CreateRet_RetNoturnoCruzaMeiaNoite_Descanso12h_Sucesso()
    {
        // RET: data=10/02, inicio=22:00, retFim real = 11/02 06:00
        // Próxima escala: 11/02 18:00 → exatamente 12h após retFim → PERMITIDO
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var hProxEscala = await DbHelper.AddHorarioAsync(ctx, new TimeOnly(18, 0), new TimeOnly(2, 0));
        await DbHelper.AddAlocacaoAsync(ctx, guarda.Id, new DateOnly(2026, 2, 11), hProxEscala);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (result, error) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "22:00", TipoRet.Mensal, null, null));

        Assert.Null(error);
        Assert.NotNull(result);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // 8. Update — não se auto-conflita no mesmo mês
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdateRet_MesmoGuardaMesmoMes_NaoConflitaConsigoProprio()
    {
        // Atualizar um RET Mensal existente não deve falhar por "já possui 1 Mensal"
        await using var ctx = DbHelper.CreateContext();
        var guarda = await DbHelper.AddGuardaAsync(ctx);
        var svc = new global::EscalaGcm.Infrastructure.Services.RetService(ctx);

        var (created, _) = await svc.CreateAsync(
            new CreateRetRequest(guarda.Id, "2026-02-10", "08:00", TipoRet.Mensal, null, "original"));
        Assert.NotNull(created);

        var updateReq = new UpdateRetRequest(guarda.Id, "2026-02-10", "09:00", TipoRet.Mensal, null, "atualizado");
        var (updated, err) = await svc.UpdateAsync(created.Id, updateReq);

        Assert.Null(err);
        Assert.NotNull(updated);
        Assert.Equal("09:00", updated.HorarioInicio);
    }
}
