using EscalaGcm.Application.DTOs.Escalas;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Application.Services.Interfaces;

public interface ISectorRuleService
{
    List<ConflictError> ValidateSectorRules(TipoSetor tipoSetor, List<AlocacaoRequest> alocacoes);
}
