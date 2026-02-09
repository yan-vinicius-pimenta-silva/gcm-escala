using EscalaGcm.Application.DTOs.Posicoes;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IPosicaoService
{
    Task<List<PosicaoDto>> GetAllAsync();
    Task<PosicaoDto?> GetByIdAsync(int id);
    Task<PosicaoDto> CreateAsync(CreatePosicaoRequest request);
    Task<PosicaoDto?> UpdateAsync(int id, UpdatePosicaoRequest request);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
