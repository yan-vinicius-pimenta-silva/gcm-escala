using EscalaGcm.Application.DTOs.Escalas;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IConflictValidationService
{
    Task<List<ConflictError>> ValidateAllocationsAsync(DateOnly data, int horarioId, List<AlocacaoRequest> alocacoes, int? excludeItemId = null);
}
