using EscalaGcm.Domain.Common;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Domain.Entities;

public class Ausencia : AuditableEntity
{
    public int GuardaId { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }
    public MotivoAusencia Motivo { get; set; }
    public string? Observacoes { get; set; }

    public Guarda Guarda { get; set; } = null!;
}
