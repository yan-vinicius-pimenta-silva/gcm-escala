namespace EscalaGcm.Application.DTOs.Ferias;

public record FeriasDto(int Id, int GuardaId, string GuardaNome, string DataInicio, string DataFim, string? Observacao);
public record CreateFeriasRequest(int GuardaId, string DataInicio, string DataFim, string? Observacao);
public record UpdateFeriasRequest(int GuardaId, string DataInicio, string DataFim, string? Observacao);
