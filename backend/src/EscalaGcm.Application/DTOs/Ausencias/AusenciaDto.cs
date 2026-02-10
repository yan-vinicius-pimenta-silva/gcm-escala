using System.ComponentModel.DataAnnotations;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Application.DTOs.Ausencias;

public record AusenciaDto(int Id, int GuardaId, string GuardaNome, string DataInicio, string DataFim, MotivoAusencia Motivo, string? Observacoes);
public record CreateAusenciaRequest(
    int GuardaId,
    [property: Required, StringLength(10)] string DataInicio,
    [property: Required, StringLength(10)] string DataFim,
    MotivoAusencia Motivo,
    [property: StringLength(144)] string? Observacoes);
public record UpdateAusenciaRequest(
    int GuardaId,
    [property: Required, StringLength(10)] string DataInicio,
    [property: Required, StringLength(10)] string DataFim,
    MotivoAusencia Motivo,
    [property: StringLength(144)] string? Observacoes);
