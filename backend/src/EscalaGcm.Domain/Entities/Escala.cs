using EscalaGcm.Domain.Common;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Domain.Entities;

public class Escala : AuditableEntity
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public int Quinzena { get; set; } // 1 or 2
    public int SetorId { get; set; }
    public StatusEscala Status { get; set; } = StatusEscala.Rascunho;

    public Setor Setor { get; set; } = null!;
    public ICollection<EscalaItem> Itens { get; set; } = [];
}
