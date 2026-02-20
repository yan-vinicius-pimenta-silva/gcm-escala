using EscalaGcm.Domain.Common;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Domain.Entities;

public class EscalaItem : AuditableEntity
{
    public int EscalaId { get; set; }
    public DateOnly Data { get; set; }
    public int TurnoId { get; set; }
    public int HorarioId { get; set; }
    public RegimeTrabalho Regime { get; set; } = RegimeTrabalho.Doze36;
    public string? Observacao { get; set; }

    public Escala Escala { get; set; } = null!;
    public Turno Turno { get; set; } = null!;
    public Horario Horario { get; set; } = null!;
    public ICollection<EscalaAlocacao> Alocacoes { get; set; } = [];
}
