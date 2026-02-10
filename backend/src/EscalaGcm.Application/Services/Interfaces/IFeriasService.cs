using EscalaGcm.Application.DTOs.Ferias;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IFeriasService
{
    Task<List<FeriasDto>> GetAllAsync();
    Task<FeriasDto?> GetByIdAsync(int id);
    Task<(FeriasDto? Result, string? Error)> CreateAsync(CreateFeriasRequest request);
    Task<(FeriasDto? Result, string? Error)> UpdateAsync(int id, UpdateFeriasRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
