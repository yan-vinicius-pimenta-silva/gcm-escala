using EscalaGcm.Application.DTOs.Escalas;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Infrastructure.Services;

public class SectorRuleService : ISectorRuleService
{
    public List<ConflictError> ValidateSectorRules(TipoSetor tipoSetor, List<AlocacaoRequest> alocacoes)
    {
        var errors = new List<ConflictError>();

        switch (tipoSetor)
        {
            case TipoSetor.CentralComunicacoes:
                if (!alocacoes.Any(a => a.EquipeId.HasValue))
                    errors.Add(new ConflictError("REGRA_SETOR", "Central de Comunicações requer uma equipe"));
                break;

            case TipoSetor.RadioPatrulha:
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Motorista))
                    errors.Add(new ConflictError("REGRA_SETOR", "Rádio Patrulha requer um Motorista"));
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Encarregado))
                    errors.Add(new ConflictError("REGRA_SETOR", "Rádio Patrulha requer um Encarregado"));
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Apoio))
                    errors.Add(new ConflictError("REGRA_SETOR", "Rádio Patrulha requer Apoio"));
                break;

            case TipoSetor.DivisaoRural:
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Motorista))
                    errors.Add(new ConflictError("REGRA_SETOR", "Divisão Rural requer um Motorista"));
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Encarregado))
                    errors.Add(new ConflictError("REGRA_SETOR", "Divisão Rural requer um Encarregado"));
                break;

            case TipoSetor.Romu:
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Motorista))
                    errors.Add(new ConflictError("REGRA_SETOR", "ROMU requer um Motorista"));
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Encarregado))
                    errors.Add(new ConflictError("REGRA_SETOR", "ROMU requer um Encarregado"));
                break;

            case TipoSetor.RondaComercio:
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Motorista))
                    errors.Add(new ConflictError("REGRA_SETOR", "Ronda Comércio requer um Motorista"));
                if (!alocacoes.Any(a => a.Funcao == FuncaoAlocacao.Encarregado))
                    errors.Add(new ConflictError("REGRA_SETOR", "Ronda Comércio requer um Encarregado"));
                break;

            case TipoSetor.Padrao:
            default:
                if (!alocacoes.Any())
                    errors.Add(new ConflictError("REGRA_SETOR", "É necessário pelo menos uma alocação"));
                break;
        }

        return errors;
    }
}
