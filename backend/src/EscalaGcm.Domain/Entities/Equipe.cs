using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Entities;

public class Equipe : AuditableEntity
{
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<EquipeMembro> Membros { get; set; } = [];
    public ICollection<EscalaAlocacao> Alocacoes { get; set; } = [];
}
