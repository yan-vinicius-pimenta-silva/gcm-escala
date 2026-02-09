using EscalaGcm.Application.DTOs.Guardas;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IGuardaService
{
    Task<List<GuardaDto>> GetAllAsync();
    Task<GuardaDto?> GetByIdAsync(int id);
    Task<GuardaDto> CreateAsync(CreateGuardaRequest request);
    Task<GuardaDto?> UpdateAsync(int id, UpdateGuardaRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
