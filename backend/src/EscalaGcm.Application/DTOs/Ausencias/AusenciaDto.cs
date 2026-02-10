using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Application.DTOs.Ausencias;

public record AusenciaDto(int Id, int GuardaId, string GuardaNome, string DataInicio, string DataFim, MotivoAusencia Motivo, string? Observacoes);
public record CreateAusenciaRequest(int GuardaId, string DataInicio, string DataFim, MotivoAusencia Motivo, string? Observacoes);
public record UpdateAusenciaRequest(int GuardaId, string DataInicio, string DataFim, MotivoAusencia Motivo, string? Observacoes);
