using EscalaGcm.Application.DTOs.Equipes;
using EscalaGcm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscalaGcm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipesController : ControllerBase
{
    private readonly IEquipeService _service;
    public EquipesController(IEquipeService service) => _service = service;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpGet("{id}")] public async Task<IActionResult> GetById(int id) { var r = await _service.GetByIdAsync(id); return r == null ? NotFound() : Ok(r); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEquipeRequest request)
    {
        var (result, error) = await _service.CreateAsync(request);
        if (error != null) return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipeRequest request)
    {
        var (result, error) = await _service.UpdateAsync(id, request);
        if (error != null) return BadRequest(new { message = error });
        return Ok(result);
    }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) { var (s, e) = await _service.DeleteAsync(id); if (!s) return BadRequest(new { message = e }); return NoContent(); }
}
