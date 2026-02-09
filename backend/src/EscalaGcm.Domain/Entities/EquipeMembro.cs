using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class EquipeMembro : AuditableEntity
{
    public int EquipeId { get; set; }
    public int GuardaId { get; set; }

    public Equipe Equipe { get; set; } = null!;
    public Guarda Guarda { get; set; } = null!;
}
