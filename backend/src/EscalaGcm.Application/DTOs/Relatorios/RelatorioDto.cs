namespace EscalaGcm.Application.DTOs.Relatorios;

public enum TipoRelatorio
{
    EscalaMensalPorSetor,
    Setores,
    Posicoes,
    Turnos,
    Horarios,
    Equipes,
    Viaturas,
    Guardas,
    Escalas,
    GuardasEscalados,
    GuardasNaoEscalados,
    Ferias,
    Ausencias,
    IndividualGuarda,
    Ret,
}

public record RelatorioRequest(
    TipoRelatorio Tipo,
    int Mes,
    int Ano,
    int? SetorId,
    int? GuardaId,
    int? MesFim,
    int? AnoFim);

public record RelatorioResult(string TituloRelatorio, List<string> Colunas, List<Dictionary<string, string>> Linhas);
