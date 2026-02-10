using EscalaGcm.Application.DTOs.Equipes;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IEquipeService
{
    Task<List<EquipeDto>> GetAllAsync();
    Task<EquipeDto?> GetByIdAsync(int id);
    Task<(EquipeDto? Result, string? Error)> CreateAsync(CreateEquipeRequest request);
    Task<(EquipeDto? Result, string? Error)> UpdateAsync(int id, UpdateEquipeRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
