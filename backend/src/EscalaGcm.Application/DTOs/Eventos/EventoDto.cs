using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Eventos;

public record EventoDto(int Id, string Nome, string DataInicio, string DataFim);
public record CreateEventoRequest(
    [Required, StringLength(144)] string Nome,
    [Required, StringLength(10)] string DataInicio,
    [Required, StringLength(10)] string DataFim);
public record UpdateEventoRequest(
    [Required, StringLength(144)] string Nome,
    [Required, StringLength(10)] string DataInicio,
    [Required, StringLength(10)] string DataFim);
