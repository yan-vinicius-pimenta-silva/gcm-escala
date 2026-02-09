using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class Ferias : AuditableEntity
{
    public int GuardaId { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }
    public string? Observacao { get; set; }

    public Guarda Guarda { get; set; } = null!;
}
