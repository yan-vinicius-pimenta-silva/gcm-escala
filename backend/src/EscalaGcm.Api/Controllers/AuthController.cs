using EscalaGcm.Application.DTOs.Auth;
using EscalaGcm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EscalaGcm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Usuário ou senha inválidos" });
        return Ok(result);
    }
}
