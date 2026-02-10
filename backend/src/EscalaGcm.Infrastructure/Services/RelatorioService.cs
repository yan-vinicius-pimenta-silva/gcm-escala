using EscalaGcm.Application.DTOs.Relatorios;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Services;

public class RelatorioService : IRelatorioService
{
    private readonly AppDbContext _context;
    public RelatorioService(AppDbContext context) => _context = context;

    public async Task<RelatorioResult> GerarRelatorioAsync(RelatorioRequest request)
    {
        return request.Tipo switch
        {
            TipoRelatorio.EscalaMensalPorSetor => await EscalaMensalPorSetor(request),
            TipoRelatorio.GuardasEscalados => await GuardasEscalados(request),
            TipoRelatorio.GuardasNaoEscalados => await GuardasNaoEscalados(request),
            TipoRelatorio.Ferias => await RelatorioFerias(request),
            TipoRelatorio.Ausencias => await RelatorioAusencias(request),
            TipoRelatorio.IndividualGuarda => await IndividualGuarda(request),
            _ => throw new ArgumentException("Tipo de relatório inválido"),
        };
    }

    private async Task<RelatorioResult> EscalaMensalPorSetor(RelatorioRequest req)
    {
        var query = _context.Escalas
            .Include(e => e.Setor)
            .Include(e => e.Itens).ThenInclude(i => i.Turno)
            .Include(e => e.Itens).ThenInclude(i => i.Horario)
            .Include(e => e.Itens).ThenInclude(i => i.Alocacoes).ThenInclude(a => a.Guarda)
            .Include(e => e.Itens).ThenInclude(i => i.Alocacoes).ThenInclude(a => a.Equipe).ThenInclude(eq => eq!.Membros).ThenInclude(m => m.Guarda)
            .Where(e => e.Ano == req.Ano && e.Mes == req.Mes);

        if (req.SetorId.HasValue) query = query.Where(e => e.SetorId == req.SetorId.Value);

        var escalas = await query.ToListAsync();
        var colunas = new List<string> { "Setor", "Quinzena", "Data", "Turno", "Horario", "Alocados", "Integrantes Equipe", "Status" };
        var linhas = new List<Dictionary<string, string>>();

        foreach (var escala in escalas)
        {
            foreach (var item in escala.Itens.OrderBy(i => i.Data))
            {
                var alocados = string.Join(", ", item.Alocacoes.Select(a =>
                    a.Guarda != null ? $"{a.Guarda.Nome} ({a.Funcao})" : a.Equipe?.Nome ?? ""));
                var integrantesEquipe = string.Join(" | ", item.Alocacoes
                    .Where(a => a.Equipe != null)
                    .Select(a => FormatEquipeIntegrantes(a.Equipe!))
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct());

                linhas.Add(new Dictionary<string, string>
                {
                    ["Setor"] = escala.Setor?.Nome ?? "",
                    ["Quinzena"] = escala.Quinzena.ToString(),
                    ["Data"] = item.Data.ToString("dd/MM/yyyy"),
                    ["Turno"] = item.Turno?.Nome ?? "",
                    ["Horario"] = item.Horario != null ? $"{item.Horario.Inicio} - {item.Horario.Fim}" : "",
                    ["Alocados"] = alocados,
                    ["Integrantes Equipe"] = integrantesEquipe,
                    ["Status"] = escala.Status.ToString(),
                });
            }
        }

        return new RelatorioResult($"Escala Mensal por Setor - {req.Mes:D2}/{req.Ano}", colunas, linhas);
    }

    private static string FormatEquipeIntegrantes(Domain.Entities.Equipe equipe)
    {
        var integrantes = equipe.Membros
            .Select(m => m.Guarda?.Nome)
            .Where(nome => !string.IsNullOrWhiteSpace(nome))
            .Cast<string>()
            .ToList();

        return integrantes.Count == 0 ? string.Empty : $"{equipe.Nome}: {string.Join(", ", integrantes)}";
    }

    private async Task<RelatorioResult> GuardasEscalados(RelatorioRequest req)
    {
        var firstDay = new DateOnly(req.Ano, req.Mes, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var alocacoes = await _context.EscalaAlocacoes
            .Include(a => a.Guarda).ThenInclude(g => g!.Posicao)
            .Include(a => a.EscalaItem).ThenInclude(i => i.Escala).ThenInclude(e => e.Setor)
            .Where(a => a.GuardaId != null && a.EscalaItem.Data >= firstDay && a.EscalaItem.Data <= lastDay)
            .ToListAsync();

        var grouped = alocacoes.GroupBy(a => a.GuardaId).Select(g => {
            var guard = g.First().Guarda!;
            var setores = g.Select(a => a.EscalaItem.Escala.Setor?.Nome ?? "").Distinct();
            return new Dictionary<string, string>
            {
                ["Guarda"] = guard.Nome,
                ["Posicao"] = guard.Posicao?.Nome ?? "",
                ["Total Plantoes"] = g.Count().ToString(),
                ["Setores"] = string.Join(", ", setores),
            };
        }).ToList();

        return new RelatorioResult(
            $"Guardas Escalados - {req.Mes:D2}/{req.Ano}",
            new List<string> { "Guarda", "Posicao", "Total Plantoes", "Setores" },
            grouped);
    }

    private async Task<RelatorioResult> GuardasNaoEscalados(RelatorioRequest req)
    {
        var firstDay = new DateOnly(req.Ano, req.Mes, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var allGuardas = await _context.Guardas
            .Include(g => g.Posicao)
            .Where(g => g.Ativo)
            .ToListAsync();

        var escaladosIds = await _context.EscalaAlocacoes
            .Where(a => a.GuardaId != null && a.EscalaItem.Data >= firstDay && a.EscalaItem.Data <= lastDay)
            .Select(a => a.GuardaId!.Value)
            .Distinct()
            .ToListAsync();

        var naoEscalados = allGuardas.Where(g => !escaladosIds.Contains(g.Id)).ToList();

        var linhas = naoEscalados.Select(g => new Dictionary<string, string>
        {
            ["Guarda"] = g.Nome,
            ["Posicao"] = g.Posicao?.Nome ?? "",
            ["Telefone"] = g.Telefone ?? "",
        }).ToList();

        return new RelatorioResult(
            $"Guardas Nao Escalados - {req.Mes:D2}/{req.Ano}",
            new List<string> { "Guarda", "Posicao", "Telefone" },
            linhas);
    }

    private async Task<RelatorioResult> RelatorioFerias(RelatorioRequest req)
    {
        var firstDay = new DateOnly(req.Ano, req.Mes, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var ferias = await _context.Ferias
            .Include(f => f.Guarda)
            .Where(f => f.DataInicio <= lastDay && f.DataFim >= firstDay)
            .OrderBy(f => f.DataInicio)
            .ToListAsync();

        var linhas = ferias.Select(f => new Dictionary<string, string>
        {
            ["Guarda"] = f.Guarda?.Nome ?? "",
            ["Inicio"] = f.DataInicio.ToString("dd/MM/yyyy"),
            ["Fim"] = f.DataFim.ToString("dd/MM/yyyy"),
            ["Observacao"] = f.Observacao ?? "",
        }).ToList();

        return new RelatorioResult(
            $"Ferias - {req.Mes:D2}/{req.Ano}",
            new List<string> { "Guarda", "Inicio", "Fim", "Observacao" },
            linhas);
    }

    private async Task<RelatorioResult> RelatorioAusencias(RelatorioRequest req)
    {
        var firstDay = new DateOnly(req.Ano, req.Mes, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var ausencias = await _context.Ausencias
            .Include(a => a.Guarda)
            .Where(a => a.DataInicio <= lastDay && a.DataFim >= firstDay)
            .OrderBy(a => a.DataInicio)
            .ToListAsync();

        var linhas = ausencias.Select(a => new Dictionary<string, string>
        {
            ["Guarda"] = a.Guarda?.Nome ?? "",
            ["Inicio"] = a.DataInicio.ToString("dd/MM/yyyy"),
            ["Fim"] = a.DataFim.ToString("dd/MM/yyyy"),
            ["Motivo"] = a.Motivo.ToString(),
            ["Observacoes"] = a.Observacoes ?? "",
        }).ToList();

        return new RelatorioResult(
            $"Ausencias - {req.Mes:D2}/{req.Ano}",
            new List<string> { "Guarda", "Inicio", "Fim", "Motivo", "Observacoes" },
            linhas);
    }

    private async Task<RelatorioResult> IndividualGuarda(RelatorioRequest req)
    {
        if (!req.GuardaId.HasValue) throw new ArgumentException("GuardaId é obrigatório para este relatório");

        var firstDay = new DateOnly(req.Ano, req.Mes, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var guarda = await _context.Guardas.FindAsync(req.GuardaId.Value);
        var guardaNome = guarda?.Nome ?? "Desconhecido";

        var alocacoes = await _context.EscalaAlocacoes
            .Include(a => a.EscalaItem).ThenInclude(i => i.Turno)
            .Include(a => a.EscalaItem).ThenInclude(i => i.Horario)
            .Include(a => a.EscalaItem).ThenInclude(i => i.Escala).ThenInclude(e => e.Setor)
            .Where(a => a.GuardaId == req.GuardaId.Value && a.EscalaItem.Data >= firstDay && a.EscalaItem.Data <= lastDay)
            .OrderBy(a => a.EscalaItem.Data)
            .ToListAsync();

        var linhas = alocacoes.Select(a => new Dictionary<string, string>
        {
            ["Data"] = a.EscalaItem.Data.ToString("dd/MM/yyyy"),
            ["Setor"] = a.EscalaItem.Escala.Setor?.Nome ?? "",
            ["Turno"] = a.EscalaItem.Turno?.Nome ?? "",
            ["Horario"] = a.EscalaItem.Horario != null ? $"{a.EscalaItem.Horario.Inicio} - {a.EscalaItem.Horario.Fim}" : "",
            ["Funcao"] = a.Funcao.ToString(),
        }).ToList();

        // Also add ferias and ausencias
        var ferias = await _context.Ferias
            .Where(f => f.GuardaId == req.GuardaId.Value && f.DataInicio <= lastDay && f.DataFim >= firstDay)
            .ToListAsync();
        foreach (var f in ferias)
        {
            linhas.Add(new Dictionary<string, string>
            {
                ["Data"] = $"{f.DataInicio:dd/MM/yyyy} a {f.DataFim:dd/MM/yyyy}",
                ["Setor"] = "",
                ["Turno"] = "FERIAS",
                ["Horario"] = "",
                ["Funcao"] = "",
            });
        }

        var ausencias = await _context.Ausencias
            .Where(a => a.GuardaId == req.GuardaId.Value && a.DataInicio <= lastDay && a.DataFim >= firstDay)
            .ToListAsync();
        foreach (var a in ausencias)
        {
            linhas.Add(new Dictionary<string, string>
            {
                ["Data"] = $"{a.DataInicio:dd/MM/yyyy} a {a.DataFim:dd/MM/yyyy}",
                ["Setor"] = "",
                ["Turno"] = $"AUSENCIA ({a.Motivo})",
                ["Horario"] = "",
                ["Funcao"] = "",
            });
        }

        return new RelatorioResult(
            $"Individual - {guardaNome} - {req.Mes:D2}/{req.Ano}",
            new List<string> { "Data", "Setor", "Turno", "Horario", "Funcao" },
            linhas);
    }
}
