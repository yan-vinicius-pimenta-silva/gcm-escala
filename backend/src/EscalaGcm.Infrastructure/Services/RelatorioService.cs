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
            TipoRelatorio.Setores => await RelatorioSetores(),
            TipoRelatorio.Posicoes => await RelatorioPosicoes(),
            TipoRelatorio.Turnos => await RelatorioTurnos(),
            TipoRelatorio.Horarios => await RelatorioHorarios(),
            TipoRelatorio.Equipes => await RelatorioEquipes(),
            TipoRelatorio.Viaturas => await RelatorioViaturas(),
            TipoRelatorio.Guardas => await RelatorioGuardas(),
            TipoRelatorio.Escalas => await RelatorioEscalas(request),
            TipoRelatorio.GuardasEscalados => await GuardasEscalados(request),
            TipoRelatorio.GuardasNaoEscalados => await GuardasNaoEscalados(request),
            TipoRelatorio.Ferias => await RelatorioFerias(request),
            TipoRelatorio.Ausencias => await RelatorioAusencias(request),
            TipoRelatorio.IndividualGuarda => await IndividualGuarda(request),
            _ => throw new ArgumentException("Tipo de relatório inválido"),
        };
    }


    private static RelatorioResult BuildCadastroRelatorio(string titulo, List<string> colunas, List<Dictionary<string, string>> linhas)
    {
        return new RelatorioResult(titulo, colunas, linhas);
    }

    private async Task<RelatorioResult> RelatorioSetores()
    {
        var linhas = await _context.Setores
            .OrderBy(s => s.Nome)
            .Select(s => new Dictionary<string, string>
            {
                ["Nome"] = s.Nome,
                ["Tipo"] = s.Tipo.ToString(),
                ["Ativo"] = s.Ativo ? "Sim" : "Nao",
            })
            .ToListAsync();

        return BuildCadastroRelatorio("Relatorio de Setores", new List<string> { "Nome", "Tipo", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioPosicoes()
    {
        var linhas = await _context.Posicoes
            .OrderBy(p => p.Nome)
            .Select(p => new Dictionary<string, string>
            {
                ["Nome"] = p.Nome,
                ["Ativo"] = p.Ativo ? "Sim" : "Nao",
            })
            .ToListAsync();

        return BuildCadastroRelatorio("Relatorio de Posicoes", new List<string> { "Nome", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioTurnos()
    {
        var linhas = await _context.Turnos
            .OrderBy(t => t.Nome)
            .Select(t => new Dictionary<string, string>
            {
                ["Nome"] = t.Nome,
                ["Ativo"] = t.Ativo ? "Sim" : "Nao",
            })
            .ToListAsync();

        return BuildCadastroRelatorio("Relatorio de Turnos", new List<string> { "Nome", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioHorarios()
    {
        var linhas = await _context.Horarios
            .OrderBy(h => h.Inicio)
            .Select(h => new Dictionary<string, string>
            {
                ["Descricao"] = h.Descricao,
                ["Inicio"] = h.Inicio.ToString(),
                ["Fim"] = h.Fim.ToString(),
                ["Ativo"] = h.Ativo ? "Sim" : "Nao",
            })
            .ToListAsync();

        return BuildCadastroRelatorio("Relatorio de Horarios", new List<string> { "Descricao", "Inicio", "Fim", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioEquipes()
    {
        var linhas = await _context.Equipes
            .OrderBy(e => e.Nome)
            .Select(e => new Dictionary<string, string>
            {
                ["Nome"] = e.Nome,
                ["Ativo"] = e.Ativo ? "Sim" : "Nao",
            })
            .ToListAsync();

        return BuildCadastroRelatorio("Relatorio de Equipes", new List<string> { "Nome", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioViaturas()
    {
        var linhas = await _context.Viaturas
            .OrderBy(v => v.Identificador)
            .Select(v => new Dictionary<string, string>
            {
                ["Identificador"] = v.Identificador,
                ["Ativo"] = v.Ativo ? "Sim" : "Nao",
            })
            .ToListAsync();

        return BuildCadastroRelatorio("Relatorio de Viaturas", new List<string> { "Identificador", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioGuardas()
    {
        var linhas = await _context.Guardas
            .Include(g => g.Posicao)
            .OrderBy(g => g.Nome)
            .Select(g => new Dictionary<string, string>
            {
                ["Nome"] = g.Nome,
                ["Posicao"] = g.Posicao.Nome,
                ["Telefone"] = g.Telefone ?? string.Empty,
                ["Ativo"] = g.Ativo ? "Sim" : "Nao",
            })
            .ToListAsync();

        return BuildCadastroRelatorio("Relatorio de Guardas", new List<string> { "Nome", "Posicao", "Telefone", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioEscalas(RelatorioRequest req)
    {
        var inicio = new DateOnly(req.Ano, req.Mes, 1);
        var fimAno = req.AnoFim ?? req.Ano;
        var fimMes = req.MesFim ?? req.Mes;
        var fim = new DateOnly(fimAno, fimMes, 1).AddMonths(1).AddDays(-1);

        if (fim < inicio)
        {
            throw new ArgumentException("Periodo final deve ser maior ou igual ao periodo inicial");
        }

        var escalas = await _context.Escalas
            .Include(e => e.Setor)
            .Include(e => e.Itens).ThenInclude(i => i.Turno)
            .Include(e => e.Itens).ThenInclude(i => i.Horario)
            .Include(e => e.Itens).ThenInclude(i => i.Alocacoes).ThenInclude(a => a.Guarda)
            .Include(e => e.Itens).ThenInclude(i => i.Alocacoes).ThenInclude(a => a.Equipe)
            .Include(e => e.Itens).ThenInclude(i => i.Alocacoes).ThenInclude(a => a.Viatura)
            .Where(e => e.Itens.Any(i => i.Data >= inicio && i.Data <= fim))
            .OrderBy(e => e.Ano).ThenBy(e => e.Mes).ThenBy(e => e.Quinzena)
            .ToListAsync();

        var linhas = new List<Dictionary<string, string>>();

        foreach (var escala in escalas)
        {
            foreach (var item in escala.Itens.Where(i => i.Data >= inicio && i.Data <= fim).OrderBy(i => i.Data))
            {
                var alocados = string.Join(", ", item.Alocacoes.Select(a =>
                    a.Guarda != null ? a.Guarda.Nome : a.Equipe?.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)));
                var viaturas = string.Join(", ", item.Alocacoes
                    .Select(a => a.Viatura?.Identificador)
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Cast<string>()
                    .Distinct());

                linhas.Add(new Dictionary<string, string>
                {
                    ["Data"] = item.Data.ToString("dd/MM/yyyy"),
                    ["Setor"] = escala.Setor?.Nome ?? string.Empty,
                    ["Turno"] = item.Turno?.Nome ?? string.Empty,
                    ["Horario"] = item.Horario != null ? $"{item.Horario.Inicio} - {item.Horario.Fim}" : string.Empty,
                    ["Alocados"] = alocados,
                    ["Viaturas"] = viaturas,
                    ["Status"] = escala.Status.ToString(),
                });
            }
        }

        return new RelatorioResult(
            $"Relatorio de Escalas - {inicio:MM/yyyy} a {fim:MM/yyyy}",
            new List<string> { "Data", "Setor", "Turno", "Horario", "Alocados", "Viaturas", "Status" },
            linhas);
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
