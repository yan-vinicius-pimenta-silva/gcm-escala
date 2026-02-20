using EscalaGcm.Application.DTOs.Guardas;
using EscalaGcm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscalaGcm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GuardasController : ControllerBase
{
    private readonly IGuardaService _service;
    private readonly IGuardaAvailabilityService _availabilityService;

    public GuardasController(IGuardaService service, IGuardaAvailabilityService availabilityService)
    {
        _service = service;
        _availabilityService = availabilityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGuardaRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGuardaRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _service.DeleteAsync(id);
        if (!success) return BadRequest(new { message = error });
        return NoContent();
    }

    [HttpGet("{id}/disponibilidade")]
    public async Task<IActionResult> GetDisponibilidade(int id, [FromQuery] int ano, [FromQuery] int mes)
    {
        if (ano <= 0 || mes < 1 || mes > 12)
            return BadRequest(new { message = "Ano e mês inválidos" });
        var result = await _availabilityService.GetAvailabilityAsync(id, ano, mes);
        return Ok(result);
    }
}
