using EscalaGcm.Application.DTOs.Relatorios;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IRelatorioService
{
    Task<RelatorioResult> GerarRelatorioAsync(RelatorioRequest request);
}
