using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class Horario : AuditableEntity
{
    public TimeOnly Inicio { get; set; }
    public TimeOnly Fim { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<EscalaItem> EscalaItens { get; set; } = [];
}
