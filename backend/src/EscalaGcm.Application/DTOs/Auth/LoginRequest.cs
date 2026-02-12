using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Auth;

public record LoginRequest(
    [Required, StringLength(144)] string NomeUsuario,
    [Required, StringLength(144)] string Senha);
public record LoginResponse(string Token, string NomeCompleto, string Perfil);
