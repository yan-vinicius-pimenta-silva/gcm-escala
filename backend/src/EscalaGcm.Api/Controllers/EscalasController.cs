using EscalaGcm.Application.DTOs.Escalas;
using EscalaGcm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscalaGcm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// REVIEW: [Authorize] without role checks. A "Consulta" user can create/delete escalas.
// Add [Authorize(Roles = "Admin,Operador")] on mutation endpoints.
[Authorize]
public class EscalasController : ControllerBase
{
    private readonly IEscalaService _service;
    public EscalasController(IEscalaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? ano, [FromQuery] int? mes, [FromQuery] int? quinzena, [FromQuery] int? setorId)
        => Ok(await _service.GetAllAsync(ano, mes, quinzena, setorId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEscalaRequest request)
    {
        var (result, error) = await _service.CreateAsync(request);
        if (error != null) return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpPost("{id}/itens")]
    public async Task<IActionResult> AddItem(int id, [FromBody] AddEscalaItemRequest request)
    {
        var (result, errors) = await _service.AddItemAsync(id, request);
        if (errors != null) return BadRequest(new { errors });
        return Ok(result);
    }

    [HttpPut("{id}/itens/{itemId}")]
    public async Task<IActionResult> UpdateItem(int id, int itemId, [FromBody] UpdateEscalaItemRequest request)
    {
        var (result, errors) = await _service.UpdateItemAsync(id, itemId, request);
        if (errors != null) return BadRequest(new { errors });
        return Ok(result);
    }

    // REVIEW: (s, e) variable names are cryptic. Use (success, error) for readability.
    [HttpDelete("{id}/itens/{itemId}")]
    public async Task<IActionResult> DeleteItem(int id, int itemId)
    {
        var (s, e) = await _service.DeleteItemAsync(id, itemId);
        if (!s) return BadRequest(new { message = e });
        return NoContent();
    }

    [HttpPost("{id}/publicar")]
    public async Task<IActionResult> Publicar(int id)
    {
        var (s, e) = await _service.PublicarAsync(id);
        if (!s) return BadRequest(new { message = e });
        return Ok(new { message = "Escala publicada com sucesso" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (s, e) = await _service.DeleteAsync(id);
        if (!s) return BadRequest(new { message = e });
        return NoContent();
    }
}
