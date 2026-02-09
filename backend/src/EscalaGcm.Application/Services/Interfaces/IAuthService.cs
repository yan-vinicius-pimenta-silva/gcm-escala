using EscalaGcm.Application.DTOs.Auth;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
