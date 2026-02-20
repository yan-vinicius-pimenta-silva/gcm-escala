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
            TipoRelatorio.Ret => await RelatorioRet(request),
            _ => throw new ArgumentException("Tipo de relatório inválido"),
        };
    }


    private static RelatorioResult BuildCadastroRelatorio(string titulo, List<string> colunas, List<Dictionary<string, string>> linhas)
    {
        return new RelatorioResult(titulo, colunas, linhas);
    }

    private async Task<RelatorioResult> RelatorioSetores()
    {
        var setores = await _context.Setores
            .OrderBy(s => s.Nome)
            .Select(s => new { s.Nome, Tipo = s.Tipo.ToString(), s.Ativo })
            .ToListAsync();

        var linhas = setores.Select(s => new Dictionary<string, string>
        {
            ["Nome"] = s.Nome,
            ["Tipo"] = s.Tipo,
            ["Ativo"] = s.Ativo ? "Sim" : "Nao",
        }).ToList();

        return BuildCadastroRelatorio("Relatorio de Setores", new List<string> { "Nome", "Tipo", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioPosicoes()
    {
        var posicoes = await _context.Posicoes
            .OrderBy(p => p.Nome)
            .Select(p => new { p.Nome, p.Ativo })
            .ToListAsync();

        var linhas = posicoes.Select(p => new Dictionary<string, string>
        {
            ["Nome"] = p.Nome,
            ["Ativo"] = p.Ativo ? "Sim" : "Nao",
        }).ToList();

        return BuildCadastroRelatorio("Relatorio de Posicoes", new List<string> { "Nome", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioTurnos()
    {
        var turnos = await _context.Turnos
            .OrderBy(t => t.Nome)
            .Select(t => new { t.Nome, t.Ativo })
            .ToListAsync();

        var linhas = turnos.Select(t => new Dictionary<string, string>
        {
            ["Nome"] = t.Nome,
            ["Ativo"] = t.Ativo ? "Sim" : "Nao",
        }).ToList();

        return BuildCadastroRelatorio("Relatorio de Turnos", new List<string> { "Nome", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioHorarios()
    {
        var horarios = await _context.Horarios
            .OrderBy(h => h.Inicio)
            .Select(h => new { h.Descricao, h.Inicio, h.Fim, h.Ativo })
            .ToListAsync();

        var linhas = horarios.Select(h => new Dictionary<string, string>
        {
            ["Descricao"] = h.Descricao,
            ["Inicio"] = h.Inicio.ToString(),
            ["Fim"] = h.Fim.ToString(),
            ["Ativo"] = h.Ativo ? "Sim" : "Nao",
        }).ToList();

        return BuildCadastroRelatorio("Relatorio de Horarios", new List<string> { "Descricao", "Inicio", "Fim", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioEquipes()
    {
        var equipes = await _context.Equipes
            .OrderBy(e => e.Nome)
            .Select(e => new { e.Nome, e.Ativo })
            .ToListAsync();

        var linhas = equipes.Select(e => new Dictionary<string, string>
        {
            ["Nome"] = e.Nome,
            ["Ativo"] = e.Ativo ? "Sim" : "Nao",
        }).ToList();

        return BuildCadastroRelatorio("Relatorio de Equipes", new List<string> { "Nome", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioViaturas()
    {
        var viaturas = await _context.Viaturas
            .OrderBy(v => v.Identificador)
            .Select(v => new { v.Identificador, v.Ativo })
            .ToListAsync();

        var linhas = viaturas.Select(v => new Dictionary<string, string>
        {
            ["Identificador"] = v.Identificador,
            ["Ativo"] = v.Ativo ? "Sim" : "Nao",
        }).ToList();

        return BuildCadastroRelatorio("Relatorio de Viaturas", new List<string> { "Identificador", "Ativo" }, linhas);
    }

    private async Task<RelatorioResult> RelatorioGuardas()
    {
        var guardas = await _context.Guardas
            .Include(g => g.Posicao)
            .OrderBy(g => g.Nome)
            .Select(g => new
            {
                g.Nome,
                Posicao = g.Posicao.Nome,
                g.Telefone,
                g.Ativo,
            })
            .ToListAsync();

        var linhas = guardas.Select(g => new Dictionary<string, string>
        {
            ["Nome"] = g.Nome,
            ["Posicao"] = g.Posicao,
            ["Telefone"] = g.Telefone ?? string.Empty,
            ["Ativo"] = g.Ativo ? "Sim" : "Nao",
        }).ToList();

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

        var items = new List<(DateOnly Data, string Setor, string Turno, string Horario, string Alocados, string Viaturas, string Status)>();

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

                items.Add((
                    item.Data,
                    escala.Setor?.Nome ?? string.Empty,
                    item.Turno?.Nome ?? string.Empty,
                    item.Horario != null ? $"{item.Horario.Inicio} - {item.Horario.Fim}" : string.Empty,
                    alocados,
                    viaturas,
                    escala.Status.ToString()
                ));
            }
        }

        var linhas = items
            .GroupBy(x => (x.Setor, x.Turno, x.Horario, x.Alocados, x.Viaturas, x.Status))
            .OrderBy(g => g.Key.Setor)
            .ThenBy(g => g.Min(x => x.Data))
            .Select(g => new Dictionary<string, string>
            {
                ["Dias"] = FormatarDatasCompletas(g.Select(x => x.Data)),
                ["Setor"] = g.Key.Setor,
                ["Turno"] = g.Key.Turno,
                ["Horario"] = g.Key.Horario,
                ["Alocados"] = g.Key.Alocados,
                ["Viaturas"] = g.Key.Viaturas,
                ["Status"] = g.Key.Status,
            })
            .ToList();

        return new RelatorioResult(
            $"Relatorio de Escalas - {inicio:MM/yyyy} a {fim:MM/yyyy}",
            new List<string> { "Dias", "Setor", "Turno", "Horario", "Alocados", "Viaturas", "Status" },
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

        var items = new List<(DateOnly Data, string Setor, string Quinzena, string Turno, string Horario, string Alocados, string IntegrantesEquipe, string Status)>();

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

                items.Add((
                    item.Data,
                    escala.Setor?.Nome ?? "",
                    escala.Quinzena.ToString(),
                    item.Turno?.Nome ?? "",
                    item.Horario != null ? $"{item.Horario.Inicio} - {item.Horario.Fim}" : "",
                    alocados,
                    integrantesEquipe,
                    escala.Status.ToString()
                ));
            }
        }

        var colunas = new List<string> { "Setor", "Quinzena", "Dias", "Turno", "Horario", "Alocados", "Integrantes Equipe", "Status" };

        var linhas = items
            .GroupBy(x => (x.Setor, x.Quinzena, x.Turno, x.Horario, x.Alocados, x.IntegrantesEquipe, x.Status))
            .OrderBy(g => g.Key.Setor)
            .ThenBy(g => g.Key.Quinzena)
            .ThenBy(g => g.Min(x => x.Data))
            .Select(g => new Dictionary<string, string>
            {
                ["Setor"] = g.Key.Setor,
                ["Quinzena"] = g.Key.Quinzena,
                ["Dias"] = FormatarDias(g.Select(x => x.Data)),
                ["Turno"] = g.Key.Turno,
                ["Horario"] = g.Key.Horario,
                ["Alocados"] = g.Key.Alocados,
                ["Integrantes Equipe"] = g.Key.IntegrantesEquipe,
                ["Status"] = g.Key.Status,
            })
            .ToList();

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

    /// <summary>
    /// Formata uma lista de datas como números de dias, agrupando consecutivos em intervalos.
    /// Ex: [1,2,3,5,6,8] → "01-03, 05-06, 08"
    /// </summary>
    private static string FormatarDias(IEnumerable<DateOnly> datas)
    {
        var dias = datas.Select(d => d.Day).OrderBy(d => d).Distinct().ToList();
        var partes = new List<string>();
        int i = 0;
        while (i < dias.Count)
        {
            int inicio = dias[i];
            int fim = inicio;
            while (i + 1 < dias.Count && dias[i + 1] == dias[i] + 1)
            {
                i++;
                fim = dias[i];
            }
            partes.Add(inicio == fim ? $"{inicio:D2}" : $"{inicio:D2}-{fim:D2}");
            i++;
        }
        return string.Join(", ", partes);
    }

    /// <summary>
    /// Formata uma lista de datas como dd/MM, agrupando consecutivos em intervalos.
    /// Usada em relatórios multi-mês. Ex: [01/02, 02/02, 05/03] → "01/02-02/02, 05/03"
    /// </summary>
    private static string FormatarDatasCompletas(IEnumerable<DateOnly> datas)
    {
        var sorted = datas.OrderBy(d => d).Distinct().ToList();
        var partes = new List<string>();
        int i = 0;
        while (i < sorted.Count)
        {
            var inicio = sorted[i];
            var fim = inicio;
            while (i + 1 < sorted.Count && sorted[i + 1] == sorted[i].AddDays(1))
            {
                i++;
                fim = sorted[i];
            }
            partes.Add(inicio == fim ? $"{inicio:dd/MM}" : $"{inicio:dd/MM}-{fim:dd/MM}");
            i++;
        }
        return string.Join(", ", partes);
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

        var escalaItems = alocacoes.Select(a => (
            Data: a.EscalaItem.Data,
            Setor: a.EscalaItem.Escala.Setor?.Nome ?? "",
            Turno: a.EscalaItem.Turno?.Nome ?? "",
            Horario: a.EscalaItem.Horario != null ? $"{a.EscalaItem.Horario.Inicio} - {a.EscalaItem.Horario.Fim}" : "",
            Funcao: a.Funcao.ToString()
        )).ToList();

        var linhas = escalaItems
            .GroupBy(x => (x.Setor, x.Turno, x.Horario, x.Funcao))
            .OrderBy(g => g.Min(x => x.Data))
            .Select(g => new Dictionary<string, string>
            {
                ["Dias"] = FormatarDias(g.Select(x => x.Data)),
                ["Setor"] = g.Key.Setor,
                ["Turno"] = g.Key.Turno,
                ["Horario"] = g.Key.Horario,
                ["Funcao"] = g.Key.Funcao,
            })
            .ToList();

        // Also add ferias and ausencias
        var ferias = await _context.Ferias
            .Where(f => f.GuardaId == req.GuardaId.Value && f.DataInicio <= lastDay && f.DataFim >= firstDay)
            .ToListAsync();
        foreach (var f in ferias)
        {
            linhas.Add(new Dictionary<string, string>
            {
                ["Dias"] = $"{f.DataInicio:dd/MM/yyyy} a {f.DataFim:dd/MM/yyyy}",
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
                ["Dias"] = $"{a.DataInicio:dd/MM/yyyy} a {a.DataFim:dd/MM/yyyy}",
                ["Setor"] = "",
                ["Turno"] = $"AUSENCIA ({a.Motivo})",
                ["Horario"] = "",
                ["Funcao"] = "",
            });
        }

        return new RelatorioResult(
            $"Individual - {guardaNome} - {req.Mes:D2}/{req.Ano}",
            new List<string> { "Dias", "Setor", "Turno", "Horario", "Funcao" },
            linhas);
    }

    private async Task<RelatorioResult> RelatorioRet(RelatorioRequest req)
    {
        var firstDay = new DateOnly(req.Ano, req.Mes, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var query = _context.Rets
            .Include(r => r.Guarda)
            .Include(r => r.Evento)
            .Where(r => r.Data >= firstDay && r.Data <= lastDay);

        if (req.GuardaId.HasValue)
            query = query.Where(r => r.GuardaId == req.GuardaId.Value);

        var rets = await query
            .OrderBy(r => r.Data).ThenBy(r => r.HorarioInicio)
            .ToListAsync();

        var linhas = rets.Select(r => new Dictionary<string, string>
        {
            ["Guarda"] = r.Guarda?.Nome ?? "",
            ["Data"] = r.Data.ToString("dd/MM/yyyy"),
            ["Inicio"] = r.HorarioInicio.ToString("HH:mm"),
            ["Fim"] = r.HorarioFim.ToString("HH:mm"),
            ["Tipo"] = r.TipoRet.ToString(),
            ["Evento"] = r.Evento?.Nome ?? "",
            ["Observacao"] = r.Observacao ?? "",
        }).ToList();

        var titulo = req.GuardaId.HasValue && rets.Count > 0
            ? $"RETs - {rets[0].Guarda?.Nome} - {req.Mes:D2}/{req.Ano}"
            : $"RETs - {req.Mes:D2}/{req.Ano}";

        return new RelatorioResult(
            titulo,
            new List<string> { "Guarda", "Data", "Inicio", "Fim", "Tipo", "Evento", "Observacao" },
            linhas);
    }
}
