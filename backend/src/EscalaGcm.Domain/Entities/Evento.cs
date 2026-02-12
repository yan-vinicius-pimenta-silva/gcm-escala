using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class Evento : AuditableEntity
{
    public string Nome { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }

    public ICollection<Ret> Rets { get; set; } = [];
}
