using EscalaGcm.Application.DTOs.Horarios;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IHorarioService
{
    Task<List<HorarioDto>> GetAllAsync();
    Task<HorarioDto?> GetByIdAsync(int id);
    Task<HorarioDto> CreateAsync(CreateHorarioRequest request);
    Task<HorarioDto?> UpdateAsync(int id, UpdateHorarioRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
