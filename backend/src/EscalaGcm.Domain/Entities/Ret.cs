using EscalaGcm.Domain.Common;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Domain.Entities;

public class Ret : AuditableEntity
{
    public int GuardaId { get; set; }
    public DateOnly Data { get; set; }
    public TimeOnly HorarioInicio { get; set; }
    public TimeOnly HorarioFim { get; set; }
    public TipoRet TipoRet { get; set; }
    public int? EventoId { get; set; }
    public string? Observacao { get; set; }

    public Guarda Guarda { get; set; } = null!;
    public Evento? Evento { get; set; }
}
