namespace EscalaGcm.Application.DTOs.Guardas;

public enum StatusDisponibilidade
{
    Disponivel,
    Parcial,
    Bloqueado
}

public record DayAvailabilityDto(
    int Dia,
    StatusDisponibilidade Status,
    string? DisponibilidadeAPartirDe,
    string? Motivo,
    string? TipoMotivo
);
