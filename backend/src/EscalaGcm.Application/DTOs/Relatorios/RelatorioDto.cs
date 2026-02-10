namespace EscalaGcm.Application.DTOs.Relatorios;

public enum TipoRelatorio
{
    EscalaMensalPorSetor,
    GuardasEscalados,
    GuardasNaoEscalados,
    Ferias,
    Ausencias,
    IndividualGuarda,
}

public record RelatorioRequest(TipoRelatorio Tipo, int Mes, int Ano, int? SetorId, int? GuardaId);

public record RelatorioResult(string TituloRelatorio, List<string> Colunas, List<Dictionary<string, string>> Linhas);
