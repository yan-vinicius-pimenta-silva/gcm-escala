using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class Viatura : AuditableEntity
{
    public string Identificador { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<EscalaAlocacao> Alocacoes { get; set; } = [];
}
