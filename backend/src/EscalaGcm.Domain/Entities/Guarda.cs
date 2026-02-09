using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class Guarda : AuditableEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public int PosicaoId { get; set; }
    public bool Ativo { get; set; } = true;

    public Posicao Posicao { get; set; } = null!;
    public ICollection<EquipeMembro> EquipeMembros { get; set; } = [];
    public ICollection<Ferias> Ferias { get; set; } = [];
    public ICollection<Ausencia> Ausencias { get; set; } = [];
    public ICollection<EscalaAlocacao> Alocacoes { get; set; } = [];
}
