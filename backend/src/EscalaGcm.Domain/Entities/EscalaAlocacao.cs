using EscalaGcm.Domain.Common;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Domain.Entities;

public class EscalaAlocacao : AuditableEntity
{
    public int EscalaItemId { get; set; }
    public int? GuardaId { get; set; }
    public int? EquipeId { get; set; }
    public FuncaoAlocacao Funcao { get; set; }
    public int? ViaturaId { get; set; }

    public EscalaItem EscalaItem { get; set; } = null!;
    public Guarda? Guarda { get; set; }
    public Equipe? Equipe { get; set; }
    public Viatura? Viatura { get; set; }
}
