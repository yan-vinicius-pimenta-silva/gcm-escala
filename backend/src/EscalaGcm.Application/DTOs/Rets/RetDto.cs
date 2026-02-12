using System.ComponentModel.DataAnnotations;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Application.DTOs.Rets;

public record RetDto(int Id, int GuardaId, string GuardaNome, string Data, string HorarioInicio, string HorarioFim, TipoRet Tipo, int? EventoId, string? EventoNome, string? Observacao);
public record CreateRetRequest(
    int GuardaId,
    [Required, StringLength(10)] string Data,
    [Required, StringLength(5)] string HorarioInicio,
    TipoRet Tipo,
    int? EventoId,
    [StringLength(144)] string? Observacao);
public record UpdateRetRequest(
    int GuardaId,
    [Required, StringLength(10)] string Data,
    [Required, StringLength(5)] string HorarioInicio,
    TipoRet Tipo,
    int? EventoId,
    [StringLength(144)] string? Observacao);
