using EscalaGcm.Domain.Common;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Domain.Entities;

public class Setor : AuditableEntity
{
    public string Nome { get; set; } = string.Empty;
    public TipoSetor Tipo { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<Escala> Escalas { get; set; } = [];
}
