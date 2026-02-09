using EscalaGcm.Application.DTOs.Setores;

namespace EscalaGcm.Application.Services.Interfaces;

public interface ISetorService
{
    Task<List<SetorDto>> GetAllAsync();
    Task<SetorDto?> GetByIdAsync(int id);
    Task<SetorDto> CreateAsync(CreateSetorRequest request);
    Task<SetorDto?> UpdateAsync(int id, UpdateSetorRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
