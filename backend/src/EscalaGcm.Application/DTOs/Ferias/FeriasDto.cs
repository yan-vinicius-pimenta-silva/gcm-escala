using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Ferias;

public record FeriasDto(int Id, int GuardaId, string GuardaNome, string DataInicio, string DataFim, string? Observacao);
public record CreateFeriasRequest(
    int GuardaId,
    [Required, StringLength(10)] string DataInicio,
    [Required, StringLength(10)] string DataFim,
    [StringLength(144)] string? Observacao);
public record UpdateFeriasRequest(
    int GuardaId,
    [Required, StringLength(10)] string DataInicio,
    [Required, StringLength(10)] string DataFim,
    [StringLength(144)] string? Observacao);
