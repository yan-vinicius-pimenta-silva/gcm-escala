using EscalaGcm.Application.DTOs.Turnos;

namespace EscalaGcm.Application.Services.Interfaces;

public interface ITurnoService
{
    Task<List<TurnoDto>> GetAllAsync();
    Task<TurnoDto?> GetByIdAsync(int id);
    Task<TurnoDto> CreateAsync(CreateTurnoRequest request);
    Task<TurnoDto?> UpdateAsync(int id, UpdateTurnoRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
