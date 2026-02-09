using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class Turno : AuditableEntity
{
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<EscalaItem> EscalaItens { get; set; } = [];
}
