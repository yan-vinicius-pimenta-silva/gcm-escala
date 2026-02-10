using EscalaGcm.Application.DTOs.Viaturas;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IViaturaService
{
    Task<List<ViaturaDto>> GetAllAsync();
    Task<ViaturaDto?> GetByIdAsync(int id);
    Task<ViaturaDto> CreateAsync(CreateViaturaRequest request);
    Task<ViaturaDto?> UpdateAsync(int id, UpdateViaturaRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
