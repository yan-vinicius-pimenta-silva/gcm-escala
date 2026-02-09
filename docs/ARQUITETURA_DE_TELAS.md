## Wireframes (arquitetura de telas) — completos, prontos para implementação

Abaixo estão os wireframes em formato textual (layout + comportamento), cobrindo todas as abas e, principalmente, a **tela de Escalas** com **quinzena** e **regras especiais**.

---

# 1) Layout geral do app

### Estrutura fixa

* **Topbar**: nome do sistema + usuário logado + perfil + sair
* **Sidebar (Abas)**:

  * Setores
  * Posições
  * Turnos
  * Horários
  * Equipes
  * Viaturas
  * Guardas
  * Escalas
  * Férias
  * Ausências
  * Relatórios
* **Área principal**: lista/forma do módulo selecionado

### Componentes padrão

* Lista com: pesquisa, ordenação, paginação
* Botão primário: “Novo”
* Ações por linha: “Visualizar”, “Editar”, “Excluir”
* Modal de confirmação para exclusão
* Toasts: sucesso/erro (mensagens claras)

---

# 2) Tela: Setores (CRUD)

## 2.1 Lista

**Cabeçalho**

* Campo busca: “Pesquisar setor…”
* Filtros: Ativo (Todos/Ativos/Inativos)
* Ordenação fixa: **A–Z**
* Botão: “Novo setor”

**Tabela**

* Nome
* Tipo (Padrão/Central/Rádio Patrulha/etc.)
* Status
* Ações: Visualizar | Editar | Excluir

## 2.2 Formulário (Novo/Editar)

* Nome (texto)
* Tipo (select)
* Ativo (switch)
* Salvar | Cancelar

Validação inline:

* Nome obrigatório
* Nome duplicado bloqueia

---

# 3) Telas CRUD padrão (Posições, Turnos, Horários, Viaturas)

## 3.1 Posições

Lista + formulário:

* Nome
* Ativo

## 3.2 Turnos

* Nome
* Ativo

## 3.3 Horários

* Início (HH:MM)
* Fim (HH:MM)
* Descrição (auto-gerada opcional)
* Ativo

Regra visual:

* Mostrar “(vira o dia)” se fim < início.

## 3.4 Viaturas

* Identificador (ex.: VTR 13400)
* Ativo

---

# 4) Tela: Guardas (CRUD)

## 4.1 Lista

* Busca por nome/telefone
* Filtro: posição / ativo
* Ordenação: A–Z

Tabela:

* Nome
* Posição
* Telefone
* Status
* Ações

## 4.2 Formulário

* Nome (obrigatório)
* Posição (select)
* Telefone (máscara)
* Ativo
* Salvar/Cancelar

---

# 5) Tela: Equipes

## 5.1 Lista

* Busca por nome
* Filtro: ativo
  Tabela:
* Nome
* Qtd. membros
* Status
* Ações (Visualizar/Editar/Excluir)

## 5.2 Formulário (Novo/Editar)

* Nome
* Ativo
* Seletor de membros (multi-select com busca):

  * Lista de guardas disponíveis (somente ativos)
  * Contador: “2/4 membros”
* Salvar/Cancelar

Validações:

* bloquear salvar com < 2 ou > 4
* mostrar erro explícito

---

# 6) Tela: Férias

## 6.1 Lista

Filtros:

* Mês/Ano (opcional)
* Guarda (opcional)
* Status (ativo por período)

Tabela:

* Guarda
* Início
* Fim
* Ações

## 6.2 Formulário

* Guarda (select com busca)
* Data início (date picker)
* Data fim (date picker)
* Observação (texto)
* Salvar/Cancelar

Validação:

* início <= fim
* bloquear sobreposição

---

# 7) Tela: Ausências

## 7.1 Lista

Filtros:

* Mês/Ano (opcional)
* Guarda (opcional)
* Motivo (opcional)

Tabela:

* Guarda
* Início
* Fim
* Motivo
* Ações

## 7.2 Formulário

* Guarda
* Data início
* Data fim
* Motivo (select)
* Observações (texto)
* Salvar/Cancelar

Validações:

* início <= fim
* bloquear sobreposição

---

# 8) Tela: Escalas (principal do produto)

## 8.1 Estrutura da tela (dividida em 3 zonas)

### Zona A — Filtros e contexto (topo)

* Setor (select com busca) [obrigatório]
* Mês (1–12) [obrigatório]
* Ano [obrigatório]
* Quinzena: (radio) **1ª (1–15)** | **2ª (16–fim)**
* Botões:

  * “Carregar”
  * “Nova escala” (se não existir)
  * “Publicar” (se status rascunho)
  * “Fechar” (opcional, trava edição)

Exibe:

* Status: Rascunho/Publicada/Fechada
* Última alteração: data e usuário

### Zona B — Montagem (painel esquerdo ou modal)

Componentes variam conforme **tipo do setor**.

Campos base (sempre):

* Turno (select)
* Horário (select)
* Seletor de dias (calendário do mês, com destaque da quinzena)

  * Botões rápidos: “Selecionar todos da quinzena”, “Limpar”
  * Dias inválidos ficam desabilitados

Depois, conforme setor:

#### (B1) Setor PADRÃO

* Guardas (multi-select com busca) — permite N guardas

#### (B2) CENTRAL DE COMUNICAÇÕES (setores 8 e 9)

* Equipe (select) — obrigatório
* (Opcional) botão “Ver membros” (abre lista 2–4)

#### (B3) RÁDIO PATRULHA (37–45)

* Equipe de apoio (select) — obrigatório (se for equipe)
* Motorista (select de guarda) — obrigatório
* Encarregado (select de guarda) — obrigatório
* (Opcional) Viatura (select) — se quiser amarrar
* (Validação) motorista ≠ encarregado

#### (B4) DIVISÃO RURAL

* Motorista (select) obrigatório
* Encarregado (select) obrigatório

#### (B5) ROMU

* Motorista obrigatório
* Encarregado obrigatório
* Terceiro integrante (select) opcional

#### (B6) RONDA COMÉRCIO

* Motorista obrigatório
* Encarregado obrigatório

Botão:

* “Adicionar à escala” (executa validações e grava)

### Zona C — Visualização (grade da escala)

Duas opções (a implementação pode ter as duas):

#### Opção 1: Grade por dia (recomendado)

* Colunas: Dias da quinzena (1–15 ou 16–fim)
* Linhas: combinações Turno + Horário (ou só Horário, dependendo do uso)
* Cada célula mostra:

  * setor
  * alocados (guardas/equipe)
  * funções (M, E, A, I)
  * ícones de conflito (se existir) — mas em regra conflitos devem ser bloqueados no salvar

Ações em cada célula:

* “Editar” (abre modal com dados)
* “Remover” (remove item/alocação)

#### Opção 2: Lista de lançamentos

Tabela:

* Data
* Turno
* Horário
* Alocados
* Observação
* Ações

---

# 9) Tela: Modal de validação (mensagens obrigatórias)

Quando o usuário clicar “Adicionar à escala”:

Se houver bloqueio, mostrar modal/alerta com lista:

* Guarda X está de férias em DD/MM/AAAA
* Guarda Y possui ausência (motivo) em DD/MM/AAAA
* Conflito: Guarda Z já está escalado em outro setor nesse dia/horário
* Equipe inválida: menos de 2 ou mais de 4 membros
* Setor Rádio Patrulha exige motorista + encarregado + apoio

Comportamento:

* nada é salvo (transação)
* manter seleção para correção

---

# 10) Tela: Relatórios

## 10.1 Filtros

* Mês (obrigatório)
* Ano (obrigatório)
* Guarda (opcional)
* Setor (opcional)
* Tipo de relatório (select):

  * Escala mensal por setor
  * Guardas escalados no mês
  * Guardas não escalados
  * Férias
  * Ausências
  * Individual do guarda

Botões:

* “Gerar”
* “Exportar Excel”
* (Opcional) “Exportar PDF”

## 10.2 Resultado

* Tabela com paginação
* Cabeçalho com parâmetros
* Totais (ex.: quantos plantões por guarda)

---

# 11) Fluxos principais (passo a passo)

## 11.1 Montar escala padrão (quinzena)

1 Selecionar setor, mês, ano, quinzena
2 Selecionar turno/horário
3 Selecionar guardas
4 Selecionar dias da quinzena
5 Adicionar à escala
6 Repetir até completar
7 Publicar

## 11.2 Rádio Patrulha

1 Selecionar Rádio Patrulha 0X
2 Selecionar motorista + encarregado
3 Selecionar equipe apoio
4 Selecionar turno/horário + dias
5 Adicionar
6 Publicar

---

# 12) Regras de UX obrigatórias (para evitar erro de operação)

* O calendário deve desabilitar dias fora da quinzena
* O sistema deve mostrar claramente o setor especial e os campos obrigatórios
* “Publicada/Fechada” deve impedir edição (ou exigir permissão admin)
* Busca de guardas/equipes deve ser rápida (autocomplete)

---

Se você quiser o próximo passo mais útil (sem entrar em código ainda), eu faço a **especificação de componentes e estados da tela de Escalas** (o “contrato” de UI), incluindo payloads e eventos (ex.: `onAddScaleItems`, `onValidateConflicts`, `onPublish`).
