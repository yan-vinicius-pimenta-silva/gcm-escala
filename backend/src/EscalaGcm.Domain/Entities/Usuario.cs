using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; } = true;
}
