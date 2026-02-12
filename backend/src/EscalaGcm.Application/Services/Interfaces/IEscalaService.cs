using EscalaGcm.Application.DTOs.Escalas;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IEscalaService
{
    Task<List<EscalaDto>> GetAllAsync(int? ano, int? mes, int? quinzena, int? setorId);
    Task<EscalaDto?> GetByIdAsync(int id);
    Task<(EscalaDto? Result, string? Error)> CreateAsync(CreateEscalaRequest request);
    Task<(EscalaItemDto? Result, List<ConflictError>? Errors)> AddItemAsync(int escalaId, AddEscalaItemRequest request);
    Task<(EscalaItemDto? Result, List<ConflictError>? Errors)> UpdateItemAsync(int escalaId, int itemId, UpdateEscalaItemRequest request);
    Task<(bool Success, string? Error)> DeleteItemAsync(int escalaId, int itemId);
    Task<(bool Success, string? Error)> PublicarAsync(int id);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
