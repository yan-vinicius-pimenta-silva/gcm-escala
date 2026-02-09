using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class Posicao : AuditableEntity
{
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<Guarda> Guardas { get; set; } = [];
}
