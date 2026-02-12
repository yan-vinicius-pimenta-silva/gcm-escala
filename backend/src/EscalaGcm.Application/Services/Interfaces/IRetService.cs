using EscalaGcm.Application.DTOs.Rets;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IRetService
{
    Task<List<RetDto>> GetAllAsync(int? guardaId = null, int? mes = null, int? ano = null);
    Task<RetDto?> GetByIdAsync(int id);
    Task<(RetDto? Result, string? Error)> CreateAsync(CreateRetRequest request);
    Task<(RetDto? Result, string? Error)> UpdateAsync(int id, UpdateRetRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
