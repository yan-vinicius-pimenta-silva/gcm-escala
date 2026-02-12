using EscalaGcm.Application.DTOs.Eventos;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IEventoService
{
    Task<List<EventoDto>> GetAllAsync();
    Task<EventoDto?> GetByIdAsync(int id);
    Task<(EventoDto? Result, string? Error)> CreateAsync(CreateEventoRequest request);
    Task<(EventoDto? Result, string? Error)> UpdateAsync(int id, UpdateEventoRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
