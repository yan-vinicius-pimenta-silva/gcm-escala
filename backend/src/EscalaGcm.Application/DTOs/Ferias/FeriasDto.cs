using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Ferias;

public record FeriasDto(int Id, int GuardaId, string GuardaNome, string DataInicio, string DataFim, string? Observacao);
public record CreateFeriasRequest(
    int GuardaId,
    [property: Required, StringLength(10)] string DataInicio,
    [property: Required, StringLength(10)] string DataFim,
    [property: StringLength(144)] string? Observacao);
public record UpdateFeriasRequest(
    int GuardaId,
    [property: Required, StringLength(10)] string DataInicio,
    [property: Required, StringLength(10)] string DataFim,
    [property: StringLength(144)] string? Observacao);
