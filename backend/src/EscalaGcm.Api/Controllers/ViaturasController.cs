using EscalaGcm.Application.DTOs.Viaturas;
using EscalaGcm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscalaGcm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ViaturasController : ControllerBase
{
    private readonly IViaturaService _service;
    public ViaturasController(IViaturaService service) => _service = service;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpGet("{id}")] public async Task<IActionResult> GetById(int id) { var r = await _service.GetByIdAsync(id); return r == null ? NotFound() : Ok(r); }
    [HttpPost] public async Task<IActionResult> Create([FromBody] CreateViaturaRequest request) { var r = await _service.CreateAsync(request); return CreatedAtAction(nameof(GetById), new { id = r.Id }, r); }
    [HttpPut("{id}")] public async Task<IActionResult> Update(int id, [FromBody] UpdateViaturaRequest request) { var r = await _service.UpdateAsync(id, request); return r == null ? NotFound() : Ok(r); }
    [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) { var (s, e) = await _service.DeleteAsync(id); if (!s) return BadRequest(new { message = e }); return NoContent(); }
}
