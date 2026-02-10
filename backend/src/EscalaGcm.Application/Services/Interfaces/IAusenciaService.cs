using EscalaGcm.Application.DTOs.Ausencias;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IAusenciaService
{
    Task<List<AusenciaDto>> GetAllAsync();
    Task<AusenciaDto?> GetByIdAsync(int id);
    Task<(AusenciaDto? Result, string? Error)> CreateAsync(CreateAusenciaRequest request);
    Task<(AusenciaDto? Result, string? Error)> UpdateAsync(int id, UpdateAusenciaRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
