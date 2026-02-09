Visão geral do produto

Aplicativo web para CRUD e gestão de escala mensal, com montagem quinzenal (1ª e 2ª quinzenas), baseado em cadastros mestres (Setores, Guardas, Equipes, Turnos, Horários, Viaturas, Posições) e com cruzamento/validação de dados obrigatório para garantir consistência (conflitos de escala x férias x ausências x duplicidades).

Requisitos do produto, requisitos técnicos (React + .NET), princípios de engenharia, restrições rígidas.

Estrutura do projeto (profissional)
1) Product Requirements (Requisitos do Produto)
1.1 Objetivos

Permitir cadastro e manutenção dos dados-base (abas).

Permitir montar, editar e registrar escalas mensais, construídas por quinzena.

Garantir integridade através de regras de cruzamento (ex.: guarda não pode ser escalado em dois setores no mesmo dia/turno/horário; não pode ser escalado em período de férias/ausência).

Gerar relatórios operacionais e gerenciais.

1.2 Usuários e papéis (RBAC)

Administrador: gerencia tudo (cadastros, escalas, relatórios, parâmetros).

Operador de Escala: cria/edita escalas, (NÃO INCLUIR: registra ocorrências (sem excluir cadastros críticos)).

Consulta: visualiza escalas e relatórios.

1.3 Abas (módulos) e funcionalidades

A) Setores

CRUD completo.

Inicialização com sua lista fixa (editável) em ordem alfabética.

Campos sugeridos: nome, ativo, tipoSetor (ex.: padrão, central_comunicacoes, radio_patrulha, especial).

B) Posições

CRUD completo (ex.: Guarda 1ª Classe, Inspetor).

Campos: nome, hierarquia, ativo.

C) Turnos

CRUD completo (ex.: Diurno, Noturno).

Campos: nome, ativo.

D) Horários

CRUD completo (ex.: 07:30–12:00).

Campos: inicio, fim, descricao, ativo.

Regra: fim pode atravessar meia-noite (se aplicável) — definir padrão.

E) Equipes

CRUD completo (ALPHA, BRAVO, etc.).

Regra: equipe com 2 a 4 guardas.

Campos: nome, ativo.

Associação: equipe ↔ guardas (membros), com vigência opcional.

F) Viaturas

CRUD completo (ex.: VTR 13400).

Campos: prefixo/identificador, ativo.

G) Guardas

CRUD completo.

Campos: nome, posicaoId, telefone, ativo.

H) Escalas (Montagem)

CRUD completo de “lançamentos de escala” (itens).

Fluxo padrão:

Entrar em Escala

Selecionar Setor

Selecionar Guarda(s) (N)

Selecionar Turno

Selecionar Horário

Selecionar Mês

Selecionar Ano

Selecionar dias (1–31) (aplicando calendário real do mês)

Salvar (gerando itens por dia)

Obrigatório: separação por 1ª quinzena (dias 1–15) e 2ª quinzena (dias 16–fim), com visualização e filtros por quinzena.

Regras especiais por setor

Centrais (8 e 9): em vez de guardas individuais, permitir escalar por Equipe (nome da equipe).

Itens de escala devem suportar “alocação por equipe”.

Rádio Patrulha (37–45):

Exigir motorista e encarregado (guardas).

Exigir equipe de apoio (dupla) vinculada (pode ser equipe ou seleção de 2 guardas, conforme sua preferência de modelagem).

Divisão Rural (18): 2 guardas (motorista + encarregado).

ROMU (46): 2 ou 3 guardas (motorista + encarregado + terceiro integrante).

Ronda Comércio (47): 2 guardas (motorista + encarregado).

I) Férias

CRUD completo.

Campos: guardaId, dataInicio, dataFim, observacao (opcional).

Regra: não permitir sobreposição de férias do mesmo guarda.

J) Ausências

CRUD completo.

Campos: guardaId, dataInicio, dataFim, motivo (enum), observacoes.

Motivos: atestado médico, doação de sangue, afastamento judicial, falta sem motivo, outros.

Regra: não permitir sobreposição de ausências do mesmo guarda.

K) Relatórios
Parâmetros:

Mês, Ano

(Opcional) Guarda

(Opcional) Setor/divisão

Saídas:

Escala mensal por setor/divisão

Guardas escalados no mês (em quaisquer divisões)

Guardas não escalados

Férias

Ausências

Relatório individual (disponibilidade e registros do guarda)

1.4 Regras de cruzamento (essenciais)

Ao salvar escala (por item/dia), bloquear:

Guarda escalado no mesmo dia em conflito de turno/horário (definir chave de conflito).

Guarda escalado durante férias.

Guarda escalado durante ausência.

Duplicidade (mesmo guarda, mesmo setor, mesmo dia, mesmo turno/horário).

Limites por equipe (2–4 membros).

Regras específicas (motorista/encarregado obrigatórios onde aplicável).

1.5 Requisitos não funcionais (produto)

Interface rápida (filtros por mês/ano/setor/quinzena).

Auditoria: rastrear quem criou/alterou/excluiu.

Exportação: PDF/Excel para relatórios (no mínimo Excel).

Acessibilidade básica (atalhos, contraste, navegação por teclado).

2) Technical Requirements (Requisitos Técnicos – React + .NET)
2.1 Stack recomendada

Frontend: React + TypeScript

React Router, React Query (ou TanStack Query)

Formulários: React Hook Form + Zod/Yup

UI: MUI/Chakra/AntD (escolher 1)

Backend: ASP.NET Core (.NET 8+), Web API

Autenticação: JWT + Refresh Token (ou cookies + OIDC se houver SSO)

ORM: Entity Framework Core

Banco de dados: PostgreSQL (recomendado) ou SQL Server

Cache/Jobs (opcional): Redis + Hangfire/Quartz (para geração de relatórios pesados)

Observabilidade: Serilog + OpenTelemetry (logs, traces)

2.2 Arquitetura

Monólito modular (recomendado para iniciar) com separação em camadas:

API (controllers)

Application (casos de uso/serviços)

Domain (entidades e regras)

Infrastructure (EF Core, repositórios, integrações)

Padrão: Clean Architecture (ou variação pragmática).

2.3 Modelagem de dados (núcleo)

Tabelas mestre

Setor(id, nome, tipo, ativo, createdAt, updatedAt)

Posicao(id, nome, ativo, ...)

Turno(id, nome, ativo, ...)

Horario(id, inicio, fim, descricao, ativo, ...)

Viatura(id, identificador, ativo, ...)

Guarda(id, nome, telefone, posicaoId, ativo, ...)

Equipe(id, nome, ativo, ...)

EquipeMembro(id, equipeId, guardaId, dataInicioVigencia?, dataFimVigencia?) (ou simples N:N se não houver vigência)

Registros de disponibilidade

Ferias(id, guardaId, dataInicio, dataFim, observacao, ...)

Ausencia(id, guardaId, dataInicio, dataFim, motivo, observacoes, ...)

Escala

EscalaCabecalho(id, mes, ano, quinzena(1|2), setorId, createdBy, ...)

EscalaItem(id, escalaCabecalhoId, data, turnoId, horarioId, tipoAlocacao(enum: Guarda|Equipe|Mista), ...)

Alocações

Para suportar casos especiais de forma flexível:

EscalaAlocacao(id, escalaItemId, guardaId?, equipeId?, funcao(enum: Motorista|Encarregado|Integrante|Apoio), viaturaId?, ...)

Assim você consegue:

Setores comuns: várias alocações do tipo Guarda.

Centrais: alocação do tipo Equipe.

Rádio Patrulha: alocações com funcao (motorista/encarregado) e uma(s) alocação(ões) de apoio.

2.4 API (exemplo de recursos)

/api/setores (GET/POST/PUT/DELETE)

/api/guardas

/api/equipes + /api/equipes/{id}/membros

/api/escalas:

POST: criar escala (cabeçalho + geração de itens por dias)

PUT: editar (regerar itens com estratégia segura)

GET: filtros por mes/ano/quinzena/setor

DELETE: exclusão controlada

/api/ferias, /api/ausencias

/api/relatorios (geração e export)

2.5 Validações e integridade

Transações no backend ao salvar escala (evitar gravação parcial).

Índices/unique constraints:

Ex.: impedir duplicidade de alocação do mesmo guarda no mesmo dia/turno/horário.

Validação de calendário: dias válidos por mês/ano (inclui bissexto).

Validação de sobreposição:

férias/ausências do mesmo guarda não podem sobrepor.

Autorização por perfil em todos os endpoints.

2.6 Relatórios

Gerar a partir de queries otimizadas (views/materialized views se necessário).

Exportação:

Excel: EPPlus/ClosedXML (atenção à licença do EPPlus; ClosedXML costuma ser mais simples).

PDF: opcional (mas comum).

3) Engineering Principles (Princípios de Engenharia)

Fonte única da verdade: regras críticas (cruzamentos) no backend.

Domínio explícito: regras modeladas no Domain/Application, não “espalhadas” na UI.

Idempotência: criação/atualização de escala deve suportar reenvio sem duplicar itens.

Auditoria e rastreabilidade: todo registro com createdAt/By, updatedAt/By.

Testabilidade:

testes unitários de regras (conflitos, setores especiais, quinzena)

testes de integração para endpoints críticos (escala).

Observabilidade: logs estruturados e correlação por request.

Evolução segura: migrations versionadas; feature flags se necessário.

4) Hard Constraints (Restrições rígidas)

Escalas são quinzenais: toda escala mensal deve ser separável por 1ª/2ª quinzena.

Cruzamento de dados é obrigatório e deve bloquear gravações inválidas:

escala x férias

escala x ausência

guarda duplicado no mesmo dia/turno/horário

Setores especiais devem seguir regras próprias (Centrais, Rádio Patrulha, Rural, ROMU, Ronda Comércio).

Equipe sempre com 2 a 4 guardas.

Relatórios devem filtrar por mês/ano e suportar filtros opcionais (guarda, setor).

Controle de acesso por perfis (admin/operador/consulta).

Persistência consistente: salvamento de escala em transação (sem registros “meio salvos”).

Dias válidos por mês/ano (evitar “31 em fevereiro”).