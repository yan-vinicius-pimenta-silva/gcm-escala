using System.ComponentModel.DataAnnotations;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Application.DTOs.Ausencias;

public record AusenciaDto(int Id, int GuardaId, string GuardaNome, string DataInicio, string DataFim, MotivoAusencia Motivo, string? Observacoes);
public record CreateAusenciaRequest(
    int GuardaId,
    [Required, StringLength(10)] string DataInicio,
    [Required, StringLength(10)] string DataFim,
    MotivoAusencia Motivo,
    [StringLength(144)] string? Observacoes);
public record UpdateAusenciaRequest(
    int GuardaId,
    [Required, StringLength(10)] string DataInicio,
    [Required, StringLength(10)] string DataFim,
    MotivoAusencia Motivo,
    [StringLength(144)] string? Observacoes);
