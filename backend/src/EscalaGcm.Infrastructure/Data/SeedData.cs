using EscalaGcm.Domain.Entities;
using EscalaGcm.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (await context.Usuarios.AnyAsync())
            return;

        // ── Usuarios ──
        context.Usuarios.Add(new Usuario
        {
            NomeUsuario = "admin",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            NomeCompleto = "Administrador",
            Perfil = PerfilUsuario.Admin,
            Ativo = true
        });
        context.Usuarios.Add(new Usuario
        {
            NomeUsuario = "operador",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("operador123"),
            NomeCompleto = "Carlos Operador",
            Perfil = PerfilUsuario.Operador,
            Ativo = true
        });
        context.Usuarios.Add(new Usuario
        {
            NomeUsuario = "consulta",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("consulta123"),
            NomeCompleto = "Maria Consulta",
            Perfil = PerfilUsuario.Consulta,
            Ativo = true
        });

        // ── Setores ──
        var setores = new List<Setor>
        {
            new() { Nome = "ADMINISTRATIVO", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "ALMOXARIFADO", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "BOMBEIROS (ADIDO)", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "CÂMARA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "CANIL", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "CARTÓRIO (P.A.T)", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "CASA DA JUVENTUDE", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "CENTRAL DE COMUNICAÇÕES OPERADOR DE COMUNICAÇÕES", Tipo = TipoSetor.CentralComunicacoes, Ativo = true },
            new() { Nome = "CENTRAL DE COMUNICAÇÕES SENTINELA/ARMEIRO", Tipo = TipoSetor.CentralComunicacoes, Ativo = true },
            new() { Nome = "CENTRO BIANCHINI", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "CENTRO POP", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "COI", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "CORREGEDORIA/GCM", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "DEFESA CIVIL", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "DELEGACIA DE POLÍCIA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "DEPARTAMENTO DE TRÂNSITO", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "DIVISÃO DE ENSINO", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "DIVISÃO RURAL", Tipo = TipoSetor.DivisaoRural, Ativo = true },
            new() { Nome = "ECOPONTO (GARAGEM DA PREFEITURA)", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "ESCALA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "ESCOLA DO CAMPO (CENTRO RURAL)", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "ESTATÍSTICA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "GABINETE", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "GANHA TEMPO", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "GUARDA PÁTIO", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "LAGO MUNICIPAL", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "LICENÇA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "LTS", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "MANUTENÇÃO VTRS", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "OUVIDORIA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "PARQUE ECOLÓGICO", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "PATRULHA ANJOS DA GUARDA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "PATRULHA MARIA DA PENHA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "PATRULHA VULNERABILIDADE SOCIAL", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "PREFEITURA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "PROCURADORIA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 01", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 02", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 03", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 04", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 05", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 06", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 07", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 08", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "RÁDIO PATRULHA 09", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "ROMU", Tipo = TipoSetor.Romu, Ativo = true },
            new() { Nome = "RONDA COMÉRCIO", Tipo = TipoSetor.RondaComercio, Ativo = true },
            new() { Nome = "SAMU", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "TIRO DE GUERRA", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "VARA DO TRABALHO", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "VEPAM", Tipo = TipoSetor.Padrao, Ativo = true },
        };
        context.Setores.AddRange(setores);

        // ── Posicoes ──
        var posInspetor = new Posicao { Nome = "Inspetor", Ativo = true };
        var posSubInspetor = new Posicao { Nome = "Sub-Inspetor", Ativo = true };
        var posGcm1 = new Posicao { Nome = "GCM 1ª Classe", Ativo = true };
        var posGcm2 = new Posicao { Nome = "GCM 2ª Classe", Ativo = true };
        var posGcm3 = new Posicao { Nome = "GCM 3ª Classe", Ativo = true };
        context.Posicoes.AddRange(posInspetor, posSubInspetor, posGcm1, posGcm2, posGcm3);

        // ── Turnos ──
        var turnoDiurno = new Turno { Nome = "Diurno", Ativo = true };
        var turnoNoturno = new Turno { Nome = "Noturno", Ativo = true };
        var turnoManha = new Turno { Nome = "Manhã", Ativo = true };
        var turnoTarde = new Turno { Nome = "Tarde", Ativo = true };
        context.Turnos.AddRange(turnoDiurno, turnoNoturno, turnoManha, turnoTarde);

        // ── Horarios ──
        var h12x36d = new Horario { Inicio = new TimeOnly(7, 0), Fim = new TimeOnly(19, 0), Descricao = "07:00 - 19:00 (12x36 Diurno)", Ativo = true };
        var h12x36n = new Horario { Inicio = new TimeOnly(19, 0), Fim = new TimeOnly(7, 0), Descricao = "19:00 - 07:00 (12x36 Noturno)", Ativo = true };
        var h8manha = new Horario { Inicio = new TimeOnly(6, 0), Fim = new TimeOnly(14, 0), Descricao = "06:00 - 14:00 (8h Manhã)", Ativo = true };
        var h8tarde = new Horario { Inicio = new TimeOnly(14, 0), Fim = new TimeOnly(22, 0), Descricao = "14:00 - 22:00 (8h Tarde)", Ativo = true };
        var h8noite = new Horario { Inicio = new TimeOnly(22, 0), Fim = new TimeOnly(6, 0), Descricao = "22:00 - 06:00 (8h Noite)", Ativo = true };
        context.Horarios.AddRange(h12x36d, h12x36n, h8manha, h8tarde, h8noite);

        // ── Viaturas ──
        var viaturas = new List<Viatura>
        {
            new() { Identificador = "VTR-001", Ativo = true },
            new() { Identificador = "VTR-002", Ativo = true },
            new() { Identificador = "VTR-003", Ativo = true },
            new() { Identificador = "VTR-004", Ativo = true },
            new() { Identificador = "VTR-005", Ativo = true },
            new() { Identificador = "VTR-006", Ativo = true },
            new() { Identificador = "VTR-007", Ativo = true },
            new() { Identificador = "VTR-008", Ativo = true },
            new() { Identificador = "MOTO-001", Ativo = true },
            new() { Identificador = "MOTO-002", Ativo = true },
            new() { Identificador = "VTR-ROMU-01", Ativo = true },
            new() { Identificador = "VTR-RURAL-01", Ativo = true },
        };
        context.Viaturas.AddRange(viaturas);

        // ── Guardas (30 guardas ficticios) ──
        var guardas = new List<Guarda>
        {
            new() { Nome = "Anderson Silva", Telefone = "(11) 99101-0001", Posicao = posInspetor, Ativo = true },
            new() { Nome = "Bruno Santos", Telefone = "(11) 99101-0002", Posicao = posSubInspetor, Ativo = true },
            new() { Nome = "Carlos Oliveira", Telefone = "(11) 99101-0003", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Daniel Pereira", Telefone = "(11) 99101-0004", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Eduardo Costa", Telefone = "(11) 99101-0005", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Fernando Almeida", Telefone = "(11) 99101-0006", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Gustavo Lima", Telefone = "(11) 99101-0007", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Henrique Souza", Telefone = "(11) 99101-0008", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Igor Ferreira", Telefone = "(11) 99101-0009", Posicao = posGcm3, Ativo = true },
            new() { Nome = "João Ribeiro", Telefone = "(11) 99101-0010", Posicao = posGcm3, Ativo = true },
            new() { Nome = "Kevin Martins", Telefone = "(11) 99101-0011", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Lucas Rodrigues", Telefone = "(11) 99101-0012", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Marcos Gomes", Telefone = "(11) 99101-0013", Posicao = posSubInspetor, Ativo = true },
            new() { Nome = "Nelson Araújo", Telefone = "(11) 99101-0014", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Oscar Barbosa", Telefone = "(11) 99101-0015", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Paulo Nascimento", Telefone = "(11) 99101-0016", Posicao = posGcm3, Ativo = true },
            new() { Nome = "Rafael Carvalho", Telefone = "(11) 99101-0017", Posicao = posInspetor, Ativo = true },
            new() { Nome = "Samuel Teixeira", Telefone = "(11) 99101-0018", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Thiago Mendes", Telefone = "(11) 99101-0019", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Ulisses Dias", Telefone = "(11) 99101-0020", Posicao = posGcm3, Ativo = true },
            new() { Nome = "Vinícius Moraes", Telefone = "(11) 99101-0021", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Wagner Batista", Telefone = "(11) 99101-0022", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Xavier Campos", Telefone = "(11) 99101-0023", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Yuri Correia", Telefone = "(11) 99101-0024", Posicao = posGcm3, Ativo = true },
            new() { Nome = "Zé Augusto", Telefone = "(11) 99101-0025", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Alex Monteiro", Telefone = "(11) 99101-0026", Posicao = posGcm1, Ativo = true },
            new() { Nome = "Breno Cardoso", Telefone = "(11) 99101-0027", Posicao = posSubInspetor, Ativo = true },
            new() { Nome = "Caio Duarte", Telefone = "(11) 99101-0028", Posicao = posGcm2, Ativo = true },
            new() { Nome = "Diego Fonseca", Telefone = "(11) 99101-0029", Posicao = posGcm3, Ativo = true },
            new() { Nome = "Elias Rocha", Telefone = "(11) 99101-0030", Posicao = posGcm1, Ativo = true },
        };
        context.Guardas.AddRange(guardas);
        await context.SaveChangesAsync();

        // ── Equipes (6 equipes de 2-4 membros) ──
        var equipes = new List<Equipe>
        {
            new() { Nome = "Equipe Alpha", Ativo = true },
            new() { Nome = "Equipe Bravo", Ativo = true },
            new() { Nome = "Equipe Charlie", Ativo = true },
            new() { Nome = "Equipe Delta", Ativo = true },
            new() { Nome = "Equipe Echo", Ativo = true },
            new() { Nome = "Equipe Foxtrot", Ativo = true },
        };
        context.Equipes.AddRange(equipes);
        await context.SaveChangesAsync();

        // Membros das equipes
        var membros = new List<EquipeMembro>
        {
            // Alpha: Anderson, Bruno, Carlos (3 membros)
            new() { EquipeId = equipes[0].Id, GuardaId = guardas[0].Id },
            new() { EquipeId = equipes[0].Id, GuardaId = guardas[1].Id },
            new() { EquipeId = equipes[0].Id, GuardaId = guardas[2].Id },
            // Bravo: Daniel, Eduardo (2 membros)
            new() { EquipeId = equipes[1].Id, GuardaId = guardas[3].Id },
            new() { EquipeId = equipes[1].Id, GuardaId = guardas[4].Id },
            // Charlie: Fernando, Gustavo, Henrique, Igor (4 membros)
            new() { EquipeId = equipes[2].Id, GuardaId = guardas[5].Id },
            new() { EquipeId = equipes[2].Id, GuardaId = guardas[6].Id },
            new() { EquipeId = equipes[2].Id, GuardaId = guardas[7].Id },
            new() { EquipeId = equipes[2].Id, GuardaId = guardas[8].Id },
            // Delta: João, Kevin, Lucas (3 membros)
            new() { EquipeId = equipes[3].Id, GuardaId = guardas[9].Id },
            new() { EquipeId = equipes[3].Id, GuardaId = guardas[10].Id },
            new() { EquipeId = equipes[3].Id, GuardaId = guardas[11].Id },
            // Echo: Marcos, Nelson (2 membros)
            new() { EquipeId = equipes[4].Id, GuardaId = guardas[12].Id },
            new() { EquipeId = equipes[4].Id, GuardaId = guardas[13].Id },
            // Foxtrot: Oscar, Paulo, Rafael (3 membros)
            new() { EquipeId = equipes[5].Id, GuardaId = guardas[14].Id },
            new() { EquipeId = equipes[5].Id, GuardaId = guardas[15].Id },
            new() { EquipeId = equipes[5].Id, GuardaId = guardas[16].Id },
        };
        context.EquipeMembros.AddRange(membros);

        // ── Ferias (6 guardas com ferias programadas) ──
        var ferias = new List<Ferias>
        {
            new() { GuardaId = guardas[2].Id, DataInicio = new DateOnly(2026, 2, 1), DataFim = new DateOnly(2026, 2, 15), Observacao = "Ferias regulares" },
            new() { GuardaId = guardas[5].Id, DataInicio = new DateOnly(2026, 2, 10), DataFim = new DateOnly(2026, 2, 28), Observacao = "Ferias de fevereiro" },
            new() { GuardaId = guardas[10].Id, DataInicio = new DateOnly(2026, 3, 1), DataFim = new DateOnly(2026, 3, 15), Observacao = "Ferias de marco" },
            new() { GuardaId = guardas[15].Id, DataInicio = new DateOnly(2026, 3, 16), DataFim = new DateOnly(2026, 3, 31), Observacao = "Ferias programadas" },
            new() { GuardaId = guardas[20].Id, DataInicio = new DateOnly(2026, 1, 5), DataFim = new DateOnly(2026, 1, 20), Observacao = "Ferias de janeiro" },
            new() { GuardaId = guardas[25].Id, DataInicio = new DateOnly(2026, 4, 1), DataFim = new DateOnly(2026, 4, 15), Observacao = "Ferias de abril" },
        };
        context.Ferias.AddRange(ferias);

        // ── Ausencias (5 guardas com ausencias) ──
        var ausencias = new List<Ausencia>
        {
            new() { GuardaId = guardas[3].Id, DataInicio = new DateOnly(2026, 2, 5), DataFim = new DateOnly(2026, 2, 7), Motivo = MotivoAusencia.AtestadoMedico, Observacoes = "Consulta medica + repouso" },
            new() { GuardaId = guardas[8].Id, DataInicio = new DateOnly(2026, 2, 12), DataFim = new DateOnly(2026, 2, 12), Motivo = MotivoAusencia.DoacaoSangue, Observacoes = "Doacao de sangue voluntaria" },
            new() { GuardaId = guardas[14].Id, DataInicio = new DateOnly(2026, 2, 18), DataFim = new DateOnly(2026, 2, 20), Motivo = MotivoAusencia.AfastamentoJudicial, Observacoes = "Audiencia judicial" },
            new() { GuardaId = guardas[22].Id, DataInicio = new DateOnly(2026, 3, 3), DataFim = new DateOnly(2026, 3, 3), Motivo = MotivoAusencia.FaltaSemMotivo },
            new() { GuardaId = guardas[28].Id, DataInicio = new DateOnly(2026, 2, 25), DataFim = new DateOnly(2026, 2, 28), Motivo = MotivoAusencia.AtestadoMedico, Observacoes = "Cirurgia eletiva" },
        };
        context.Ausencias.AddRange(ausencias);

        await context.SaveChangesAsync();
    }
}
