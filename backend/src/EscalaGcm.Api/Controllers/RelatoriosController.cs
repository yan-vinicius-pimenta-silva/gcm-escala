using EscalaGcm.Application.DTOs.Relatorios;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscalaGcm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _service;
    public RelatoriosController(IRelatorioService service) => _service = service;

    [HttpPost("gerar")]
    public async Task<IActionResult> Gerar([FromBody] RelatorioRequest request)
    {
        var result = await _service.GerarRelatorioAsync(request);
        return Ok(result);
    }

    [HttpPost("excel")]
    public async Task<IActionResult> GerarExcel([FromBody] RelatorioRequest request)
    {
        var result = await _service.GerarRelatorioAsync(request);
        var bytes = ExcelReportGenerator.Generate(result);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"relatorio_{request.Tipo}_{request.Mes:D2}_{request.Ano}.xlsx");
    }
}
