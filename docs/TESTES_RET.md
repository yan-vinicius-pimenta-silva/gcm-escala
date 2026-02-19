# Plano de Testes — RET (Regime Especial de Trabalho)

> Baseado na especificação `RET.md` e na implementação em `RetService.cs`, `retSchema.ts` e demais arquivos relacionados.

---

## Sumário

1. [Backend — RetService: Duração e cálculo do horário fim](#1-backend--retservice-duração-e-cálculo-do-horário-fim)
2. [Backend — RetService: Validação do evento (RET Evento)](#2-backend--retservice-validação-do-evento-ret-evento)
3. [Backend — RetService: Limite mensal](#3-backend--retservice-limite-mensal)
4. [Backend — RetService: Descanso antes do RET](#4-backend--retservice-descanso-antes-do-ret)
5. [Backend — RetService: Descanso depois do RET](#5-backend--retservice-descanso-depois-do-ret)
6. [Backend — RetService: Intervalo total de 32h](#6-backend--retservice-intervalo-total-de-32h)
7. [Backend — RetService: Turnos noturnos](#7-backend--retservice-turnos-noturnos)
8. [Backend — RetService: CRUD básico](#8-backend--retservice-crud-básico)
9. [Backend — API: Endpoints HTTP](#9-backend--api-endpoints-http)
10. [Backend — EventoService: CRUD e vínculos](#10-backend--eventoservice-crud-e-vínculos)
11. [Frontend — retSchema: Validação Zod](#11-frontend--retschema-validação-zod)
12. [Frontend — RetForm: Comportamento do formulário](#12-frontend--retform-comportamento-do-formulário)
13. [Frontend — RetsPage: Tabela e ações](#13-frontend--retspage-tabela-e-ações)
14. [Frontend — Relatório de RET](#14-frontend--relatório-de-ret)
15. [Cenários End-to-End da Spec](#15-cenários-end-to-end-da-spec)

---

## Convenções

| Símbolo | Significado |
|---------|-------------|
| ✅ VÁLIDO | Operação deve ser permitida |
| ❌ BLOQUEADO | Operação deve ser rejeitada com mensagem de erro |
| `→` | Resultado esperado |

Variáveis de contexto usadas nos testes:
- **Guarda A** = guarda ativo qualquer
- **Turno Diurno** = Horario com Inicio=06:00, Fim=18:00
- **Turno Noturno** = Horario com Inicio=18:00, Fim=06:00 (cruza meia-noite)
- **Carnaval** = Evento com DataInicio=14/02/2026, DataFim=17/02/2026

---

## 1. Backend — RetService: Duração e cálculo do horário fim

A duração é sempre **8 horas fixas**, calculada pelo servidor a partir do `HorarioInicio`. O cliente nunca envia `HorarioFim`.

| # | Entrada | Esperado |
|---|---------|----------|
| 1.1 | `HorarioInicio = "08:00"` | `HorarioFim = "16:00"` salvo no banco |
| 1.2 | `HorarioInicio = "00:00"` | `HorarioFim = "08:00"` |
| 1.3 | `HorarioInicio = "18:00"` | `HorarioFim = "02:00"` (cruza meia-noite) |
| 1.4 | `HorarioInicio = "22:00"` | `HorarioFim = "06:00"` (cruza meia-noite) |
| 1.5 | `HorarioInicio = "16:00"` | `HorarioFim = "00:00"` |
| 1.6 | Request com campo `HorarioFim` inventado | Campo ignorado; fim recalculado como Inicio + 8h |

---

## 2. Backend — RetService: Validação do evento (RET Evento)

Regra da spec (passo 2 do fluxo): *"RET Evento e não há evento no mês? → BLOQUEAR"*

O evento vinculado precisa ter seu período **sobreposto ao mês do RET**. A data do RET **não precisa** estar dentro do período do evento.

### 2a. EventoId ausente ou inválido

| # | Cenário | Esperado |
|---|---------|----------|
| 2.1 | Tipo = `Evento`, `eventoId = null` | ❌ BLOQUEADO `"RET de Evento requer um evento vinculado"` |
| 2.2 | Tipo = `Evento`, `eventoId = 9999` (não existe no banco) | ❌ BLOQUEADO `"Evento não encontrado"` |
| 2.3 | Tipo = `Mensal`, `eventoId = null` | ✅ VÁLIDO (sem verificação de evento para Mensal) |
| 2.4 | Tipo = `Mensal`, `eventoId = 1` (evento existente) | ✅ VÁLIDO (campo ignorado na validação para Mensal) |

### 2b. Evento no mês correto

| # | Cenário | Esperado |
|---|---------|----------|
| 2.5 | RET em 19/02, evento Carnaval 14–17/02 (**Cenário Válido 3 da spec**) | ✅ VÁLIDO — evento está em fevereiro, RET também |
| 2.6 | RET em 14/02, evento Carnaval 14–17/02 (data dentro do evento) | ✅ VÁLIDO |
| 2.7 | RET em 01/02, evento Carnaval 14–17/02 (data antes do evento, mesmo mês) | ✅ VÁLIDO |
| 2.8 | RET em 28/02, evento Carnaval 14–17/02 (data após o evento, mesmo mês) | ✅ VÁLIDO |

### 2c. Evento em mês diferente

| # | Cenário | Esperado |
|---|---------|----------|
| 2.9 | RET em 01/03, evento Carnaval 14–17/02 | ❌ BLOQUEADO `"O evento selecionado não ocorre no mês do RET"` |
| 2.10 | RET em 31/01, evento Carnaval 14–17/02 | ❌ BLOQUEADO |
| 2.11 | RET em 15/03, evento de março (01–31/03) | ✅ VÁLIDO |

### 2d. Evento que cruza mês

| # | Cenário | Esperado |
|---|---------|----------|
| 2.12 | RET em 28/02, evento com DataInicio=25/02, DataFim=03/03 | ✅ VÁLIDO (evento sobrepõe fevereiro) |
| 2.13 | RET em 01/03, evento com DataInicio=25/02, DataFim=03/03 | ✅ VÁLIDO (evento sobrepõe março) |
| 2.14 | RET em 04/03, evento com DataInicio=25/02, DataFim=03/03 | ❌ BLOQUEADO (evento encerrou em março, mas antes do dia 4) |

---

## 3. Backend — RetService: Limite mensal

Regra: **máx 1 RET Mensal + máx 1 RET Evento = máx 2 RETs por mês**.

### 3a. Limite de RET Mensal

| # | Cenário | Esperado |
|---|---------|----------|
| 3.1 | Guarda A sem RETs em fev/2026 → cria RET Mensal em 11/02 | ✅ VÁLIDO |
| 3.2 | Guarda A com 1 RET Mensal em fev/2026 → tenta criar 2º RET Mensal em 25/02 (**Cenário Inválido 3**) | ❌ BLOQUEADO `"O guarda já possui 1 RET Mensal neste mês"` |
| 3.3 | Guarda A com RET Mensal em fev/2026 → cria RET Mensal em mar/2026 | ✅ VÁLIDO (mês diferente) |
| 3.4 | Guarda A sem RETs → cria RET Mensal em jan, outro em fev, outro em mar | ✅ VÁLIDO (um por mês cada) |

### 3b. Limite de RET Evento

| # | Cenário | Esperado |
|---|---------|----------|
| 3.5 | Guarda A sem RETs de Evento em fev/2026 → cria RET Evento (Carnaval) em 19/02 | ✅ VÁLIDO |
| 3.6 | Guarda A com 1 RET Evento em fev/2026 → tenta criar 2º RET Evento no mesmo mês | ❌ BLOQUEADO `"O guarda já possui 1 RET de Evento neste mês"` |

### 3c. Combinação mensal + evento

| # | Cenário | Esperado |
|---|---------|----------|
| 3.7 | Guarda A: RET Mensal em 11/02 + RET Evento em 19/02 (**Cenário Válido 3**) | ✅ VÁLIDO (total = 2 = máximo) |
| 3.8 | Guarda A: RET Mensal + RET Evento + tenta 3º RET qualquer tipo | ❌ BLOQUEADO pelo limite do tipo específico |

### 3d. Edição não conta o próprio RET

| # | Cenário | Esperado |
|---|---------|----------|
| 3.9 | Guarda A com único RET Mensal em fev/2026 → edita esse mesmo RET | ✅ VÁLIDO (excludeId exclui o próprio ID do count) |
| 3.10 | Guarda A com RET Mensal em 11/02 → edita mudando a data para 15/02 (mesmo mês, mesmo tipo) | ✅ VÁLIDO |
| 3.11 | Guarda A com RET Mensal em 11/02 e RET Evento em 19/02 → edita o RET Mensal | ✅ VÁLIDO (excludeId correto) |

### 3e. Isolamento entre guardas

| # | Cenário | Esperado |
|---|---------|----------|
| 3.12 | Guarda A com RET Mensal em fev → Guarda B cria RET Mensal no mesmo mês | ✅ VÁLIDO (limite por guarda, não global) |

---

## 4. Backend — RetService: Descanso antes do RET

Regra: se houver escala **terminando antes do RET**, o intervalo deve ser **≥ 12h**.

| # | Cenário | Esperado |
|---|---------|----------|
| 4.1 | Guarda A sem nenhuma escala anterior → cria RET qualquer dia | ✅ VÁLIDO |
| 4.2 | Escala (Turno Diurno) em 10/02, termina 18:00 → RET em 11/02 às 06:00 (12h de descanso) | ✅ VÁLIDO |
| 4.3 | Escala termina 18:00 no dia X → RET às 06:01 do dia X+1 (12h01 de descanso) | ✅ VÁLIDO |
| 4.4 | Escala termina 18:00 no dia X → RET às 05:59 do dia X+1 (11h59 de descanso) | ❌ BLOQUEADO `"Descanso insuficiente antes do RET: 11.9h (mínimo 12h)"` |
| 4.5 | Escala termina 18:00 no dia X → RET às 02:00 do dia X+1 (8h de descanso) (**Cenário Inválido 1**) | ❌ BLOQUEADO `"Descanso insuficiente antes do RET: 8.0h (mínimo 12h)"` |
| 4.6 | Escala termina 18:00 no dia X → RET às 18:01 do dia X (1min de descanso) | ❌ BLOQUEADO |
| 4.7 | Escala termina 18:00 exato → RET inicia 18:00 exato (0h de descanso) | ❌ BLOQUEADO |

---

## 5. Backend — RetService: Descanso depois do RET

Regra: se houver escala **iniciando após o RET**, o intervalo deve ser **≥ 12h**.

| # | Cenário | Esperado |
|---|---------|----------|
| 5.1 | Guarda A sem nenhuma escala posterior → cria RET qualquer dia | ✅ VÁLIDO |
| 5.2 | RET termina 16:00 dia X → próxima escala inicia 04:00 dia X+1 (12h de descanso) | ✅ VÁLIDO |
| 5.3 | RET termina 16:00 dia X → próxima escala inicia 04:01 dia X+1 (12h01) | ✅ VÁLIDO |
| 5.4 | RET termina 16:00 dia X → próxima escala inicia 03:59 dia X+1 (11h59) | ❌ BLOQUEADO `"Descanso insuficiente após o RET: 11.9h (mínimo 12h)"` |
| 5.5 | RET termina 16:00 dia X → próxima escala inicia 20:00 dia X (4h descanso) | ❌ BLOQUEADO `"Descanso insuficiente após o RET: 4.0h (mínimo 12h)"` |
| 5.6 | RET termina 16:00 → próxima escala inicia 16:00 exato (0h) | ❌ BLOQUEADO |

---

## 6. Backend — RetService: Intervalo total de 32h

Regra: quando há escala **antes E depois** do RET, o intervalo total (fim da escala anterior até início da próxima) deve ser **≥ 32h**.

| # | Cenário | Esperado |
|---|---------|----------|
| 6.1 | Escala termina 18:00/10 → RET 06:00–14:00/11 → escala inicia 02:00/12 (intervalo = 32h) | ✅ VÁLIDO (**Cenário Válido 1 da spec**) |
| 6.2 | Intervalo total = 32h01 | ✅ VÁLIDO |
| 6.3 | Intervalo total = 31h59 | ❌ BLOQUEADO `"Intervalo total entre escalas insuficiente: 31.9h (mínimo 32h)"` |
| 6.4 | Escala antes mas sem escala depois | Não verifica intervalo total (apenas descanso antes) |
| 6.5 | Escala depois mas sem escala antes | Não verifica intervalo total (apenas descanso depois) |
| 6.6 | Sem nenhuma escala adjacente | Nenhuma verificação de descanso |

---

## 7. Backend — RetService: Turnos noturnos

O turno noturno tem `Horario.Fim < Horario.Inicio` (ex: 18:00–06:00), significando que o turno **termina no dia seguinte**. O serviço deve calcular o `Fim` real adicionando 1 dia.

| # | Cenário | Esperado |
|---|---------|----------|
| 7.1 | Turno Noturno (18:00–06:00) em 05/02 → fim real = 06/02 06:00. RET em 06/02 às 18:00 (12h de descanso) | ✅ VÁLIDO |
| 7.2 | Turno Noturno em 05/02 → fim real = 06/02 06:00. RET em 06/02 às 08:00 (2h descanso) | ❌ BLOQUEADO |
| 7.3 | Turno Noturno em 05/02 → fim real = 06/02 06:00. RET em 07/02 qualquer hora | ✅ VÁLIDO (distância > 12h) |
| 7.4 | Turno Diurno (06:00–18:00) em 10/02 não confundido com noturno | ✅ `fim = 10/02 18:00` (sem adição de dia) |
| 7.5 | RET noturno: inicia 22:00/11/02, fim calculado = 06:00/12/02 → próxima escala inicia 18:00/12/02 (12h após) | ✅ VÁLIDO |
| 7.6 | RET noturno: inicia 22:00/11/02, fim calculado = 06:00/12/02 → próxima escala inicia 10:00/12/02 (4h após) | ❌ BLOQUEADO |
| 7.7 | Escala Turno Noturno em 02/02 (termina 03/02 06:00). RET em 04/02 08:00 (26h de descanso) | ✅ VÁLIDO (como no exemplo GCM Silva da spec) |

---

## 8. Backend — RetService: CRUD básico

### GetAllAsync

| # | Cenário | Esperado |
|---|---------|----------|
| 8.1 | Sem filtros, 3 RETs no banco | Retorna lista com 3 itens, ordenados por Data desc, HorarioInicio asc |
| 8.2 | `guardaId = 2`, RETs de guardas 1 e 2 | Retorna apenas RETs do guarda 2 |
| 8.3 | `mes = 2, ano = 2026` | Retorna apenas RETs de fevereiro/2026 |
| 8.4 | `guardaId + mes + ano` combinados | Filtra pelo guarda no mês/ano |
| 8.5 | Banco vazio | Retorna lista vazia `[]` |
| 8.6 | DTO retornado contém `guardaNome` e `eventoNome` | Nomes carregados via Include |

### GetByIdAsync

| # | Cenário | Esperado |
|---|---------|----------|
| 8.7 | ID existente | Retorna `RetDto` com todos os campos |
| 8.8 | ID inexistente | Retorna `null` |

### CreateAsync

| # | Cenário | Esperado |
|---|---------|----------|
| 8.9 | Dados válidos (RET Mensal) | Retorna `(RetDto, null)`, registro persistido |
| 8.10 | Dados válidos (RET Evento com evento no mês) | Retorna `(RetDto, null)` com `EventoNome` preenchido |
| 8.11 | Validação falha (2º RET Mensal) | Retorna `(null, "O guarda já possui 1 RET Mensal neste mês")` |
| 8.12 | `HorarioFim` no DTO retornado = `HorarioInicio + 8h` | Duração sempre 8h |

### UpdateAsync

| # | Cenário | Esperado |
|---|---------|----------|
| 8.13 | ID inexistente | Retorna `(null, "RET não encontrado")` |
| 8.14 | Dados válidos | Retorna `(RetDto atualizado, null)` |
| 8.15 | Atualizar data permanecendo no mesmo mês | Não bloqueia a si mesmo no limit check |
| 8.16 | Alterar tipo de Mensal para Evento | Valida evento no mês |

### DeleteAsync

| # | Cenário | Esperado |
|---|---------|----------|
| 8.17 | ID existente | Retorna `(true, null)`, registro removido |
| 8.18 | ID inexistente | Retorna `(false, "RET não encontrado")` |

---

## 9. Backend — API: Endpoints HTTP

Todos os endpoints exigem JWT Bearer token.

| # | Request | Status esperado | Body esperado |
|---|---------|-----------------|---------------|
| 9.1 | `GET /api/rets` sem token | `401 Unauthorized` | — |
| 9.2 | `GET /api/rets` autenticado | `200 OK` | `[ RetDto, ... ]` |
| 9.3 | `GET /api/rets?guardaId=1&mes=2&ano=2026` | `200 OK` | Lista filtrada |
| 9.4 | `GET /api/rets/1` (existe) | `200 OK` | `RetDto` |
| 9.5 | `GET /api/rets/9999` (não existe) | `404 Not Found` | — |
| 9.6 | `POST /api/rets` com body válido | `201 Created` | `RetDto` com `id` gerado |
| 9.7 | `POST /api/rets` com validação falha | `400 Bad Request` | `{ "message": "<motivo>" }` |
| 9.8 | `POST /api/rets` sem `guardaId` | `400 Bad Request` | Erro de model binding |
| 9.9 | `PUT /api/rets/1` com body válido | `200 OK` | `RetDto` atualizado |
| 9.10 | `PUT /api/rets/9999` | `400 Bad Request` | `{ "message": "RET não encontrado" }` |
| 9.11 | `DELETE /api/rets/1` | `204 No Content` | — |
| 9.12 | `DELETE /api/rets/9999` | `400 Bad Request` | `{ "message": "RET não encontrado" }` |

---

## 10. Backend — EventoService: CRUD e vínculos

| # | Cenário | Esperado |
|---|---------|----------|
| 10.1 | Criar evento com DataInicio ≤ DataFim | ✅ VÁLIDO |
| 10.2 | Criar evento com DataInicio > DataFim | ❌ BLOQUEADO (datas inválidas) |
| 10.3 | Listar eventos | Retorna todos os eventos |
| 10.4 | Deletar evento **sem** RETs vinculados | ✅ VÁLIDO |
| 10.5 | Deletar evento **com** RETs vinculados | ❌ BLOQUEADO (integridade referencial) |
| 10.6 | DTO retornado contém `id`, `nome`, `dataInicio`, `dataFim` | Formato correto |

---

## 11. Frontend — retSchema: Validação Zod

### Campos obrigatórios

| # | Campo | Entrada | Esperado |
|---|-------|---------|----------|
| 11.1 | `guardaId` | `0` | Erro `"Guarda é obrigatório"` |
| 11.2 | `guardaId` | `-1` | Erro (min 1) |
| 11.3 | `guardaId` | `1` | ✅ Válido |
| 11.4 | `data` | `""` | Erro `"Data é obrigatória"` |
| 11.5 | `data` | `"2026-02-11"` | ✅ Válido |
| 11.6 | `horarioInicio` | `""` | Erro `"Horário de início é obrigatório"` |
| 11.7 | `horarioInicio` | `"08:00"` | ✅ Válido |
| 11.8 | `tipo` | `""` | Erro `"Tipo é obrigatório"` |
| 11.9 | `tipo` | `"Mensal"` | ✅ Válido |
| 11.10 | `tipo` | `"Evento"` | ✅ Válido (sem exigir eventoId ainda) |

### Validação cruzada: tipo Evento exige eventoId

| # | Tipo | eventoId | Esperado |
|---|------|----------|----------|
| 11.11 | `"Evento"` | `undefined` | Erro `"Evento é obrigatório para RET de Evento"` no campo `eventoId` |
| 11.12 | `"Evento"` | `null` | Erro |
| 11.13 | `"Evento"` | `1` | ✅ Válido |
| 11.14 | `"Mensal"` | `undefined` | ✅ Válido (eventoId não obrigatório para Mensal) |
| 11.15 | `"Mensal"` | `1` | ✅ Válido (eventoId opcional para Mensal) |

### Campos opcionais

| # | Campo | Entrada | Esperado |
|---|-------|---------|----------|
| 11.16 | `observacao` | `undefined` | ✅ Válido |
| 11.17 | `observacao` | `"Texto"` | ✅ Válido |
| 11.18 | `eventoId` | `undefined` (tipo Mensal) | ✅ Válido |

---

## 12. Frontend — RetForm: Comportamento do formulário

### Exibição inicial

| # | Cenário | Esperado |
|---|---------|----------|
| 12.1 | Abrir formulário para novo RET | Campos: Guarda (autocomplete), Data, Horário Início, Horário Fim (desabilitado), Tipo (select), Observação |
| 12.2 | Campo Horário Fim | Sempre desabilitado, não editável pelo usuário |
| 12.3 | Autocomplete de Evento | **NÃO** visível quando tipo = "Mensal" |
| 12.4 | Autocomplete de Evento | **VISÍVEL** quando tipo = "Evento" |
| 12.5 | Autocomplete de Guarda | Lista apenas guardas com `ativo = true` |
| 12.6 | Título do dialog | "Novo RET" para criação, "Editar RET" para edição |

### Cálculo do Horário Fim (client-side)

| # | HorarioInicio | HorarioFim exibido |
|---|---------------|--------------------|
| 12.7 | `08:00` | `16:00` |
| 12.8 | `00:00` | `08:00` |
| 12.9 | `18:00` | `02:00` |
| 12.10 | `22:00` | `06:00` |
| 12.11 | `""` (não preenchido) | `""` (campo vazio) |

### Mudança de tipo limpa eventoId

| # | Ação | Esperado |
|---|------|----------|
| 12.12 | Tipo = "Evento" → seleciona evento X → muda tipo para "Mensal" | `eventoId` é limpo (`undefined`) |
| 12.13 | Tipo = "Mensal" → muda para "Evento" | Autocomplete de evento aparece vazio |

### Edição

| # | Cenário | Esperado |
|---|---------|----------|
| 12.14 | Abrir formulário de edição com `editData` preenchido | Todos os campos pré-preenchidos com os valores do RET |
| 12.15 | RET de Evento editado mostra evento correto selecionado | Autocomplete com evento do `editData.eventoId` |
| 12.16 | Fechar e reabrir para novo RET | Campos resetados para valores vazios |

### Submit

| # | Cenário | Esperado |
|---|---------|----------|
| 12.17 | Submit com dados válidos | Chama `onSubmit` com `RetFormData` correto |
| 12.18 | Submit tipo "Evento" sem evento selecionado | Exibe erro no campo Evento, não chama `onSubmit` |
| 12.19 | Submit sem guarda selecionada | Exibe erro no campo Guarda, não chama `onSubmit` |
| 12.20 | Botão "Cancelar" | Fecha dialog sem chamar `onSubmit` |

---

## 13. Frontend — RetsPage: Tabela e ações

### Tabela

| # | Cenário | Esperado |
|---|---------|----------|
| 13.1 | Carregar página com RETs no banco | Tabela exibe colunas: ID, Guarda, Data, Início, Fim, Tipo, Evento, Observação, Ações |
| 13.2 | Coluna Data | Exibida no formato `dd/mm/yyyy` (não `yyyy-mm-dd`) |
| 13.3 | RET sem evento vinculado | Coluna Evento exibe vazio |
| 13.4 | RET com evento vinculado | Coluna Evento exibe o nome do evento |
| 13.5 | Banco vazio | Tabela exibe mensagem de "sem registros" ou tabela vazia |

### Ações

| # | Ação | Esperado |
|---|------|----------|
| 13.6 | Clicar botão "Adicionar" (PageHeader) | Abre RetForm com campos vazios, título "Novo RET" |
| 13.7 | Clicar ícone editar em linha | Abre RetForm com dados da linha, título "Editar RET" |
| 13.8 | Clicar ícone excluir em linha | Abre ConfirmDialog com confirmação |
| 13.9 | Confirmar exclusão | Chama `DELETE /api/rets/{id}`, exibe snackbar "RET excluído", remove da tabela |
| 13.10 | Cancelar exclusão | Não chama API, dialog fecha |
| 13.11 | Criar RET com sucesso | Exibe snackbar "RET criado", linha aparece na tabela |
| 13.12 | Atualizar RET com sucesso | Exibe snackbar "RET atualizado", linha atualizada |
| 13.13 | Erro de validação na API | Exibe snackbar com a mensagem de erro retornada pelo servidor |

---

## 14. Frontend — Relatório de RET

### Filtros

| # | Cenário | Esperado |
|---|---------|----------|
| 14.1 | Dropdown "Tipo de Relatório" | Contém opção `"RETs (Regime Especial de Trabalho)"` |
| 14.2 | Selecionar tipo "Ret" | Exibe filtro `"Guarda (opcional)"` |
| 14.3 | Tipo "Ret" sem guarda selecionada | Botão "Gerar" **habilitado** (guarda é opcional para RETs) |
| 14.4 | Tipo "IndividualGuarda" sem guarda | Botão "Gerar" **desabilitado** (guarda obrigatória para Individual) |
| 14.5 | Tipo "Ret" com guarda selecionada | Botão "Gerar" habilitado |

### Resultado da grid

| # | Cenário | Esperado |
|---|---------|----------|
| 14.6 | Gerar relatório de RETs de fev/2026 | Colunas: Guarda, Data, Inicio, Fim, Tipo, Evento, Observacao |
| 14.7 | Gerar sem guarda | Retorna todos os RETs do mês/ano |
| 14.8 | Gerar com guarda específica | Retorna apenas RETs daquele guarda |
| 14.9 | Mês sem RETs | Exibe mensagem "Não há registros" |
| 14.10 | RET Mensal sem evento | Coluna Evento vazia |
| 14.11 | RET Evento com Carnaval | Coluna Evento = "Carnaval 2026" (nome do evento) |

### Exportação

| # | Cenário | Esperado |
|---|---------|----------|
| 14.12 | Clicar "Exportar Excel" com tipo "Ret" | Download de arquivo `.xlsx` com os dados |
| 14.13 | Clicar "Exportar PDF" com tipo "Ret" | Download de arquivo `.pdf` com os dados |
| 14.14 | Arquivo exportado contém cabeçalho com título | Ex: `"RETs - 02/2026"` ou `"RETs - GCM Silva - 02/2026"` |

---

## 15. Cenários End-to-End da Spec

Reprodução direta dos cenários descritos no `RET.md`.

---

### ✅ CENÁRIO VÁLIDO 1 — RET entre duas escalas (32h total)

**Setup:**
- Guarda A com escala (Turno Diurno 06:00–18:00) em 10/02/2026
- Guarda A com escala iniciando em 12/02/2026 às 02:00

**Ação:** Criar RET Mensal em 11/02/2026 às 06:00

**Verificações:**
- Escala anterior termina 10/02 18:00 → descanso antes = 12h ✅
- RET dura 8h: 06:00–14:00 ✅
- Próxima escala inicia 12/02 02:00 → descanso depois = 12h ✅
- Intervalo total (10/02 18:00 → 12/02 02:00) = 32h ✅
- **Resultado:** ✅ RET criado com sucesso

---

### ✅ CENÁRIO VÁLIDO 2 — RET sem escala anterior

**Setup:**
- Guarda A sem nenhuma escala antes de 11/02/2026
- Guarda A com escala iniciando em 12/02/2026 às 02:00

**Ação:** Criar RET Mensal em 11/02/2026 às 06:00

**Verificações:**
- Sem escala antes → nenhuma verificação de descanso antes ✅
- RET dura 8h: 06:00–14:00 ✅
- Descanso depois = 12h ✅
- **Resultado:** ✅ RET criado com sucesso

---

### ✅ CENÁRIO VÁLIDO 3 — 2 RETs no mês (mensal + evento)

**Setup:**
- Evento Carnaval cadastrado: 14/02/2026 → 17/02/2026
- Guarda A sem RETs em fevereiro/2026

**Ação 1:** Criar RET Mensal em 11/02/2026
- **Resultado:** ✅ VÁLIDO

**Ação 2:** Criar RET Evento (Carnaval) em **19/02/2026** (após o término do Carnaval)
- Verificação: evento existe e está em fevereiro ✅
- Verificação: RET Mensal já existe, mas este é tipo Evento ✅
- **Resultado:** ✅ VÁLIDO

**Estado final:** 2 RETs em fevereiro (1 Mensal + 1 Evento) = máximo permitido

---

### ❌ CENÁRIO INVÁLIDO 1 — Descanso insuficiente antes do RET

**Setup:**
- Guarda A com escala terminando SEG 10/02/2026 às 18:00

**Ação:** Criar RET em TER 11/02/2026 às **02:00** (apenas 8h de descanso)

**Verificações:**
- Descanso antes = 8h < 12h → BLOQUEADO
- **Resultado:** ❌ `"Descanso insuficiente antes do RET: 8.0h (mínimo 12h)"`

---

### ❌ CENÁRIO INVÁLIDO 2 — Duração incorreta (impossível via sistema)

**Verificação:**
- O DTO de criação não possui campo `HorarioFim`
- O campo `HorarioFim` é calculado exclusivamente pelo servidor como `HorarioInicio + 8h`
- **Resultado:** Não é possível criar um RET com duração diferente de 8h via API

---

### ❌ CENÁRIO INVÁLIDO 3 — Mais de 1 RET Mensal no mês

**Setup:**
- Guarda A com RET Mensal em 11/02/2026

**Ação:** Criar 2º RET Mensal em 25/02/2026

**Verificações:**
- Count de RETs Mensais do guarda em fev/2026 = 1 → BLOQUEADO
- **Resultado:** ❌ `"O guarda já possui 1 RET Mensal neste mês"`

---

## Resumo de cobertura

| Área | Qtd. de casos |
|------|--------------|
| Duração e horário fim | 6 |
| Validação do evento | 14 |
| Limite mensal | 12 |
| Descanso antes | 7 |
| Descanso depois | 6 |
| Intervalo total 32h | 6 |
| Turnos noturnos | 7 |
| CRUD do serviço | 12 |
| Endpoints HTTP | 12 |
| EventoService | 6 |
| Schema Zod (frontend) | 18 |
| RetForm (frontend) | 20 |
| RetsPage (frontend) | 13 |
| Relatório de RET | 14 |
| Cenários E2E da spec | 5 |
| **Total** | **158** |
