namespace EscalaGcm.Application.DTOs.Auth;

public record LoginRequest(string NomeUsuario, string Senha);
public record LoginResponse(string Token, string NomeCompleto, string Perfil);
