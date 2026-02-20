# LÓGICA DE DISPONIBILIDADE DE DIAS NA ABA ESCALA

## 1. OBJETIVO

Calcular automaticamente quais dias do mês estão disponíveis para escalação de um guarda, baseado no regime 12x36 (12 horas de trabalho + 36 horas de folga), bloqueando/liberando botões de dias no calendário conforme a disponibilidade real.

---

## 2. PADRONIZAÇÕES NECESSÁRIAS

### 2.1 Turnos (apenas 2 opções)
```
• DIURNO
• NOTURNO
```

### 2.2 Horários (tipo TIME no banco de dados)
```
Armazenamento: TIME (HH:MM:SS)

Exemplos comuns:
• 06:00 - 18:00  (Diurno - 12h)
• 07:00 - 19:00  (Diurno - 12h)
• 18:00 - 06:00  (Noturno - 12h, atravessa meia-noite)
• 19:00 - 07:00  (Noturno - 12h, atravessa meia-noite)
```

**Importante:** Horários noturnos atravessam a meia-noite (iniciam em um dia e terminam no dia seguinte).

---

## 3. REGIMES DE TRABALHO

O sistema suporta **2 tipos de regime**, selecionáveis pelo escalante:

### 3.1 REGIME 12x36 (12 horas de trabalho + 36 horas de folga)

**Definição:**
- **12 horas:** Tempo trabalhado
- **36 horas:** Tempo de folga obrigatória
- **48 horas:** Ciclo completo (12 + 36)

**Fórmula de Cálculo:**
```
Início do Trabalho → Fim do Trabalho (+12h) → Fim da Folga (+36h) → Próxima Disponibilidade
```

**Exemplo prático:**
```
Trabalho: 16/02 às 07:00 até 16/02 às 19:00 (12h)
Folga: 16/02 às 19:00 até 18/02 às 07:00 (36h)
Próxima disponibilidade: 18/02 às 07:00
```

**Quando usar:**
- Turnos fixos de 12 horas
- Patrulhamento 24 horas
- Postos que exigem cobertura contínua

---

### 3.2 REGIME 8 HORAS DIÁRIAS

**Definição:**
- **8 horas:** Tempo trabalhado por dia
- **Sem folga obrigatória calculada:** Sistema não aplica regra de 36h
- **Dias consecutivos:** Guarda pode trabalhar dias seguidos

**Fórmula de Cálculo:**
```
Início do Trabalho → Fim do Trabalho (+8h) → Disponível no mesmo dia (após jornada)
```

**Exemplo prático:**
```
Trabalho: 16/02 às 08:00 até 16/02 às 16:00 (8h)
Próxima disponibilidade: 16/02 às 16:00 (mesmo dia, após expediente)
                        OU 17/02 às 08:00 (dia seguinte, mesma hora)
```

**Quando usar:**
- Expediente administrativo
- Postos fixos em horário comercial
- Funções que não exigem regime de plantão
- Escalas de segunda a sexta

**Diferença chave:**
```
┌────────────────────────────────────────────────────────┐
│ REGIME 12x36                                           │
│ DIA 16: Trabalha 12h                                  │
│ DIA 17: BLOQUEADO (folga obrigatória)                │
│ DIA 18: Disponível (após 36h de folga)               │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ REGIME 8 HORAS DIÁRIAS                                │
│ DIA 16: Trabalha 8h                                   │
│ DIA 17: DISPONÍVEL (pode trabalhar novamente)        │
│ DIA 18: DISPONÍVEL (pode trabalhar novamente)        │
└────────────────────────────────────────────────────────┘
```

---

## 4. REGRA PRINCIPAL DE DISPONIBILIDADE

### 4.1 Regra Fundamental (aplica-se aos 2 regimes)
**"Um dia só fica BLOQUEADO se o guarda estiver ocupado (trabalhando ou de folga obrigatória) pelas 24 horas COMPLETAS daquele dia."**

### 4.2 Diferenças por Regime

#### REGIME 12x36
```
- Calcula 36h de folga obrigatória após cada jornada
- Bloqueia dias que estejam 100% dentro do período de folga
- Libera dias parcialmente livres com aviso de horário
```

#### REGIME 8 HORAS DIÁRIAS
```
- NÃO calcula folga obrigatória
- Libera dia imediatamente após fim da jornada
- Permite trabalhar dias consecutivos
- Bloqueia apenas horários conflitantes no mesmo dia
```

### 4.3 Consequências da Regra

**Se o dia tem horas livres:**
- ✅ Botão do dia DISPONÍVEL

**Se o dia está 100% ocupado (apenas 12x36):**
- ❌ Botão do dia BLOQUEADO

**Se o dia fica livre apenas a partir de determinada hora:**
- ✅ Botão do dia DISPONÍVEL
- ⚠️ Exibe aviso ao selecionar horário conflitante
- ✅ Permite salvar se horário selecionado for após a liberação

---

## 5. EXEMPLOS DETALHADOS

### 5.1 EXEMPLO 1: Turno Diurno (07:00 - 19:00)

**Escala selecionada:**
```
Dia: 16/02
Horário: 07:00 às 19:00
```

**Cálculo automático do sistema:**
```
┌─────────────────────────────────────────────────┐
│ TRABALHO                                        │
│ Início: 16/02 às 07:00                         │
│ Fim:    16/02 às 19:00 (12 horas depois)       │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ FOLGA (36 HORAS)                                │
│ Início: 16/02 às 19:00                         │
│ Fim:    18/02 às 07:00 (36 horas depois)       │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PRÓXIMA DISPONIBILIDADE                         │
│ A partir de: 18/02 às 07:00                    │
└─────────────────────────────────────────────────┘
```

**Análise dia a dia:**

```
┌────────────────────────────────────────────────────────┐
│ DIA 16/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 06:59  →  LIVRE (7 horas)                    │
│ 07:00 - 19:00  →  TRABALHANDO (12 horas)             │
│ 19:00 - 23:59  →  FOLGA (5 horas)                    │
│                                                        │
│ Status: NÃO ocupa 24h completas                       │
│ ✅ BOTÃO DISPONÍVEL                                   │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ DIA 17/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 23:59  →  FOLGA (24 horas COMPLETAS)         │
│                                                        │
│ Status: Ocupa 24h completas                           │
│ ❌ BOTÃO BLOQUEADO                                    │
│ Motivo: "Guarda em período de folga obrigatória"     │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ DIA 18/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 06:59  →  FOLGA (7 horas)                    │
│ 07:00 - 23:59  →  LIVRE (17 horas)                   │
│                                                        │
│ Status: NÃO ocupa 24h completas                       │
│ ✅ BOTÃO DISPONÍVEL (a partir das 07:00)             │
│                                                        │
│ Se usuário tentar adicionar escala antes das 07:00:  │
│ ⚠️ AVISO: "Guarda disponível apenas a partir das     │
│           07:00. Período de folga até 06:59."        │
│ ✅ Mas botão continua clicável                        │
└────────────────────────────────────────────────────────┘
```

---

### 5.2 EXEMPLO 2: Turno Noturno (19:00 - 07:00) - Atravessa Meia-Noite

**Escala selecionada:**
```
Dia: 16/02
Horário: 19:00 às 07:00
```

**Cálculo automático do sistema:**
```
┌─────────────────────────────────────────────────┐
│ TRABALHO (atravessa meia-noite)                 │
│ Início: 16/02 às 19:00                         │
│ Fim:    17/02 às 07:00 (12 horas depois)       │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ FOLGA (36 HORAS)                                │
│ Início: 17/02 às 07:00                         │
│ Fim:    18/02 às 19:00 (36 horas depois)       │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PRÓXIMA DISPONIBILIDADE                         │
│ A partir de: 18/02 às 19:00                    │
└─────────────────────────────────────────────────┘
```

**Análise dia a dia:**

```
┌────────────────────────────────────────────────────────┐
│ DIA 16/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 18:59  →  LIVRE (19 horas)                   │
│ 19:00 - 23:59  →  TRABALHANDO (5 horas)              │
│                                                        │
│ Status: NÃO ocupa 24h completas                       │
│ ✅ BOTÃO DISPONÍVEL                                   │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ DIA 17/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 06:59  →  TRABALHANDO (7 horas)              │
│ 07:00 - 23:59  →  FOLGA (17 horas)                   │
│                                                        │
│ Status: NÃO ocupa 24h completas                       │
│ ✅ BOTÃO DISPONÍVEL (a partir das 07:00)             │
│                                                        │
│ Se usuário tentar adicionar escala antes das 07:00:  │
│ ⚠️ AVISO: "Guarda ainda em jornada de trabalho até   │
│           06:59. Disponível a partir das 07:00."     │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ DIA 18/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 18:59  →  FOLGA (19 horas)                   │
│ 19:00 - 23:59  →  LIVRE (5 horas)                    │
│                                                        │
│ Status: NÃO ocupa 24h completas                       │
│ ✅ BOTÃO DISPONÍVEL (a partir das 19:00)             │
│                                                        │
│ Se usuário tentar adicionar escala antes das 19:00:  │
│ ⚠️ AVISO: "Guarda em período de folga até 18:59.     │
│           Disponível a partir das 19:00."            │
└────────────────────────────────────────────────────────┘
```

---

### 5.3 EXEMPLO 3: Folga Termina no Meio do Dia (15:00)

**Escala selecionada:**
```
Dia: 16/02
Horário: 03:00 às 15:00
```

**Cálculo automático do sistema:**
```
┌─────────────────────────────────────────────────┐
│ TRABALHO                                        │
│ Início: 16/02 às 03:00                         │
│ Fim:    16/02 às 15:00 (12 horas depois)       │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ FOLGA (36 HORAS)                                │
│ Início: 16/02 às 15:00                         │
│ Fim:    18/02 às 03:00 (36 horas depois)       │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PRÓXIMA DISPONIBILIDADE                         │
│ A partir de: 18/02 às 03:00                    │
└─────────────────────────────────────────────────┘
```

**Análise dia a dia:**

```
┌────────────────────────────────────────────────────────┐
│ DIA 16/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 02:59  →  LIVRE (3 horas)                    │
│ 03:00 - 15:00  →  TRABALHANDO (12 horas)             │
│ 15:00 - 23:59  →  FOLGA (9 horas)                    │
│                                                        │
│ Status: NÃO ocupa 24h completas                       │
│ ✅ BOTÃO DISPONÍVEL                                   │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ DIA 17/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 23:59  →  FOLGA (24 horas COMPLETAS)         │
│                                                        │
│ Status: Ocupa 24h completas                           │
│ ❌ BOTÃO BLOQUEADO                                    │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ DIA 18/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 02:59  →  FOLGA (3 horas)                    │
│ 03:00 - 23:59  →  LIVRE (21 horas)                   │
│                                                        │
│ Status: NÃO ocupa 24h completas                       │
│ ✅ BOTÃO DISPONÍVEL (a partir das 03:00)             │
│                                                        │
│ Se usuário tentar adicionar escala antes das 03:00:  │
│ ⚠️ AVISO: "Guarda disponível apenas a partir das     │
│           03:00. Período de folga até 02:59."        │
└────────────────────────────────────────────────────────┘
```

---

### 5.4 EXEMPLO 4: Regime 8 Horas Diárias (08:00 - 16:00)

**Escala selecionada:**
```
Dia: 16/02
Horário: 08:00 às 16:00
Regime: 8 HORAS DIÁRIAS
```

**Cálculo automático do sistema:**
```
┌─────────────────────────────────────────────────┐
│ TRABALHO                                        │
│ Início: 16/02 às 08:00                         │
│ Fim:    16/02 às 16:00 (8 horas depois)        │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ SEM FOLGA OBRIGATÓRIA                           │
│ Sistema NÃO calcula período de 36h             │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PRÓXIMA DISPONIBILIDADE                         │
│ Mesmo dia: 16/02 às 16:00 (imediato)          │
│ Dia seguinte: 17/02 às 08:00 (pode trabalhar) │
└─────────────────────────────────────────────────┘
```

**Análise dia a dia:**

```
┌────────────────────────────────────────────────────────┐
│ DIA 16/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 07:59  →  LIVRE (8 horas)                    │
│ 08:00 - 16:00  →  TRABALHANDO (8 horas)              │
│ 16:00 - 23:59  →  LIVRE (8 horas)                    │
│                                                        │
│ Status: NÃO ocupa 24h completas                       │
│ ✅ BOTÃO DISPONÍVEL                                   │
│                                                        │
│ 💡 Guarda pode ter OUTRA escala no mesmo dia:        │
│    - Antes das 08:00 ❌ (conflito)                   │
│    - Após as 16:00 ✅ (permitido)                    │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ DIA 17/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 23:59  →  LIVRE (24 horas COMPLETAS)         │
│                                                        │
│ Status: Totalmente disponível                         │
│ ✅ BOTÃO DISPONÍVEL                                   │
│                                                        │
│ 💡 Guarda PODE trabalhar dias consecutivos:          │
│    - Segunda: 08:00-16:00 ✅                          │
│    - Terça: 08:00-16:00 ✅                            │
│    - Quarta: 08:00-16:00 ✅                           │
│    - (regime 8h não tem folga obrigatória)           │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ DIA 18/02                                              │
├────────────────────────────────────────────────────────┤
│ 00:00 - 23:59  →  LIVRE (24 horas COMPLETAS)         │
│                                                        │
│ Status: Totalmente disponível                         │
│ ✅ BOTÃO DISPONÍVEL                                   │
└────────────────────────────────────────────────────────┘
```

**Comparação Visual: 12x36 vs 8h Diárias**

```
REGIME 12x36 - Dia 16 às 07:00-19:00
┌─────────────────────────────────────────────────┐
│ 16/02 │ Trabalha 12h                           │
│ 17/02 │ ❌ BLOQUEADO (folga obrigatória)       │
│ 18/02 │ ✅ Disponível (após 36h folga)         │
└─────────────────────────────────────────────────┘

REGIME 8 HORAS - Dia 16 às 08:00-16:00
┌─────────────────────────────────────────────────┐
│ 16/02 │ Trabalha 8h                            │
│ 17/02 │ ✅ DISPONÍVEL (pode trabalhar)         │
│ 18/02 │ ✅ DISPONÍVEL (pode trabalhar)         │
└─────────────────────────────────────────────────┘
```

---

## 6. FLUXO DE USO NA INTERFACE

### 6.1 Passo a Passo do Usuário

```
┌─────────────────────────────────────────────────┐
│ PASSO 1: Selecionar Guarda                     │
│ Dropdown: GCM João Silva                       │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PASSO 2: Selecionar Regime de Trabalho         │
│ Radio: ⚫ 12x36  ⚪ 8h Diárias                  │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PASSO 3: Selecionar Turno                      │
│ Radio: ⚫ Diurno  ⚪ Noturno                    │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PASSO 4: Selecionar Horário                    │
│ Dropdown: 07:00 - 19:00  (se 12x36)           │
│           08:00 - 16:00  (se 8h diárias)      │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PASSO 5: Selecionar Primeiro Dia               │
│ Calendário: Clicar no dia 16                   │
│                                                 │
│ ⚙️ SISTEMA CALCULA AUTOMATICAMENTE:            │
│                                                 │
│ SE REGIME 12x36:                               │
│   - Fim do trabalho: 16/02 19:00              │
│   - Fim da folga: 18/02 07:00 (36h depois)    │
│   - Próxima disponibilidade: 18/02 07:00      │
│                                                 │
│ SE REGIME 8H DIÁRIAS:                          │
│   - Fim do trabalho: 16/02 16:00              │
│   - Próxima disponibilidade: 16/02 16:00      │
│     (ou 17/02 08:00 para novo expediente)     │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│ PASSO 6: Calendário Atualiza Visualmente       │
│                                                 │
│ Fevereiro 2026 - REGIME 12x36                 │
│ D  S  T  Q  Q  S  S                           │
│                                                 │
│ 16 ✅ (disponível - dia selecionado)          │
│ 17 ❌ (bloqueado - folga 24h)                 │
│ 18 ✅ (disponível - livre após 07:00)         │
│ 19 ⚪ (disponível - totalmente livre)         │
│                                                 │
│ Fevereiro 2026 - REGIME 8H DIÁRIAS            │
│ D  S  T  Q  Q  S  S                           │
│                                                 │
│ 16 ✅ (disponível - dia selecionado)          │
│ 17 ✅ (disponível - pode trabalhar)           │
│ 18 ✅ (disponível - pode trabalhar)           │
│ 19 ✅ (disponível - pode trabalhar)           │
└─────────────────────────────────────────────────┘
```

---

## 7. COMPORTAMENTO DOS BOTÕES NO CALENDÁRIO

### 7.1 Estados Visuais

```
┌───────────────────────────────────────────────────────┐
│ LEGENDA DO CALENDÁRIO                                 │
├───────────────────────────────────────────────────────┤
│                                                        │
│ ✅ DIA DISPONÍVEL (verde)                             │
│    - Guarda totalmente livre OU                       │
│    - Guarda tem horas livres no dia                   │
│    - Clicável                                         │
│                                                        │
│ ❌ DIA BLOQUEADO (vermelho/cinza)                     │
│    - Guarda ocupado 24h completas                     │
│    - Não clicável                                     │
│    - Cursor: not-allowed                              │
│                                                        │
│ ⚠️ DIA COM RESTRIÇÃO (amarelo)                        │
│    - Guarda disponível após determinada hora          │
│    - Clicável                                         │
│    - Tooltip: "Disponível a partir das XX:XX"        │
│                                                        │
│ 🔵 DIA SELECIONADO (azul)                             │
│    - Dia já adicionado à escala                       │
│    - Clicável (para remover)                          │
└───────────────────────────────────────────────────────┘
```

### 7.2 Tooltips Informativos

Ao passar o mouse sobre cada dia:

```
DIA DISPONÍVEL TOTAL:
"✅ Disponível - Guarda livre neste dia"

DIA BLOQUEADO:
"❌ Indisponível - Guarda em período de folga obrigatória (12x36)"

DIA PARCIALMENTE DISPONÍVEL:
"⚠️ Disponível a partir das 15:00 - Folga até 14:59"

DIA JÁ SELECIONADO:
"🔵 Dia já adicionado - Clique para remover"
```

---

## 8. VALIDAÇÕES E AVISOS

### 8.1 Validação ao Selecionar Horário Conflitante

**Cenário:**
- Dia 18 está disponível a partir das 07:00
- Usuário tenta adicionar horário 06:00 - 18:00

**Comportamento:**
```
┌──────────────────────────────────────────────────┐
│ ⚠️ ATENÇÃO: CONFLITO COM PERÍODO DE FOLGA       │
├──────────────────────────────────────────────────┤
│                                                   │
│ Guarda disponível apenas a partir das 07:00     │
│ no dia 18/02.                                    │
│                                                   │
│ Horário selecionado: 06:00 - 18:00              │
│ Conflito: 06:00 - 06:59 (1 hora)                │
│                                                   │
│ Período de folga obrigatória até 06:59          │
│ (regime 12x36).                                  │
│                                                   │
│ ⚙️ Sugestões:                                    │
│ • Alterar horário para 07:00 - 19:00            │
│ • Escolher outro dia                             │
│ • Continuar (sistema bloqueará ao salvar)       │
│                                                   │
│ [Alterar Horário]  [Escolher Outro Dia]  [X]    │
└──────────────────────────────────────────────────┘
```

### 8.2 Validação ao Salvar

**Antes de salvar, sistema verifica TODOS os dias selecionados:**

```
✅ VALIDAÇÕES QUE PASSAM:
- Todos os horários respeitam período de folga
- Nenhum dia tem conflito com escalas existentes
- Descanso de 12h respeitado (quando aplicável RET)

❌ VALIDAÇÕES QUE BLOQUEIAM:
- Dia com horário que invade período de folga
- Dia com conflito com outra escala já existente
- Dia com conflito com férias cadastradas
- Dia com conflito com ausências registradas
```

---

## 9. CRUZAMENTO DE DADOS

### 9.1 Dados Verificados Automaticamente

```
┌─────────────────────────────────────────────────┐
│ CRUZAMENTO AUTOMÁTICO AO CALCULAR               │
│ DISPONIBILIDADE                                 │
├─────────────────────────────────────────────────┤
│                                                  │
│ 1. Escalas Existentes do Guarda                 │
│    - Verifica todas escalas já cadastradas      │
│    - Bloqueia dias/horários ocupados            │
│                                                  │
│ 2. Período de Folga (12x36)                     │
│    - Calcula automaticamente 36h após trabalho  │
│    - Bloqueia dias totalmente em folga          │
│    - Avisa dias parcialmente em folga           │
│                                                  │
│ 3. Férias Cadastradas                           │
│    - Bloqueia todos os dias em período de férias│
│    - Tooltip: "❌ Guarda em período de férias"  │
│                                                  │
│ 4. Ausências Registradas                        │
│    - Bloqueia dias com ausências                │
│    - Tooltip: "❌ Guarda ausente (motivo)"      │
│                                                  │
│ 5. RET (Regime Especial de Trabalho)           │
│    - Verifica RET existente                     │
│    - Aplica regra das 32h se aplicável          │
│    - Bloqueia conflitos                         │
└─────────────────────────────────────────────────┘
```

### 9.2 Mensagens de Bloqueio por Tipo

```
ESCALA EXISTENTE:
"❌ Dia indisponível - Guarda já escalado em Setor 02 
    (18:00-06:00)"

FOLGA 12x36:
"❌ Dia indisponível - Período de folga obrigatória 
    (trabalhou 16/02 07:00-19:00)"

FÉRIAS:
"❌ Dia indisponível - Guarda em período de férias 
    (10/02 a 24/02)"

AUSÊNCIA:
"❌ Dia indisponível - Guarda ausente 
    (Motivo: Atestado Médico)"

RET:
"❌ Dia indisponível - Aguardando descanso após RET 
    (disponível a partir de 18/02 15:00)"
```

---

## 10. REGRAS DE NEGÓCIO CONSOLIDADAS

### 10.1 Regimes de Trabalho
- ✅ **12x36:** 12h trabalho + 36h folga obrigatória
- ✅ **8h Diárias:** 8h trabalho + sem folga obrigatória
- ✅ Escalante seleciona o regime ao criar escala
- ✅ Sistema calcula disponibilidade conforme regime escolhido

### 10.2 Regime 12x36
- ✅ 12 horas de trabalho
- ✅ 36 horas de folga obrigatória
- ✅ Sistema calcula automaticamente
- ❌ Bloqueia dias ocupados 24h pela folga

### 10.3 Regime 8 Horas Diárias
- ✅ 8 horas de trabalho
- ✅ Sem folga obrigatória calculada
- ✅ Permite trabalhar dias consecutivos
- ✅ Libera mesmo dia após término da jornada

### 10.4 Bloqueio de Dias
- ❌ Dia ocupado 24h completas → BLOQUEADO (apenas 12x36)
- ✅ Dia com horas livres → DISPONÍVEL (ambos regimes)
- ⚠️ Dia livre após determinada hora → DISPONÍVEL com AVISO

### 10.5 Horários
- ✅ Armazenados como TIME (precisão de minuto)
- ✅ Suportam atravessar meia-noite
- ✅ Validação automática de conflitos
- ✅ Duração automática conforme regime (12h ou 8h)

### 10.6 Turnos
- ✅ DIURNO: normalmente 06:00-18:00 ou 07:00-19:00 (12x36) ou 08:00-16:00 (8h)
- ✅ NOTURNO: normalmente 18:00-06:00 ou 19:00-07:00 (12x36)
- ✅ Apenas 2 opções (simplificado)

### 10.7 Cruzamento de Dados (ambos regimes)
- ✅ Verifica escalas existentes
- ✅ Verifica férias
- ✅ Verifica ausências
- ✅ Verifica folga 12x36 (quando aplicável)
- ✅ Verifica descanso RET (quando aplicável)

---

## 11. BENEFÍCIOS DESTA IMPLEMENTAÇÃO

### 11.1 Para o Sistema
- ✅ Impossível criar escalas conflitantes
- ✅ Respeito automático ao regime 12x36
- ✅ Dados sempre consistentes
- ✅ Auditoria completa de disponibilidade

### 11.2 Para o Usuário (Escalante)
- ✅ Interface visual clara (botões coloridos)
- ✅ Feedback imediato de disponibilidade
- ✅ Avisos preventivos antes de salvar
- ✅ Redução drástica de erros de escalação
- ✅ Agilidade na montagem da escala

### 11.3 Para o Guarda
- ✅ Garantia de folga obrigatória (12x36)
- ✅ Impossível ser escalado em período de férias
- ✅ Impossível ser escalado em período de ausência
- ✅ Descanso adequado respeitado
- ✅ Conformidade trabalhista

---

## 12. RESUMO EXECUTIVO

### O que muda?
**Antes:** Escalante digitava manualmente os dias, sem validação automática de folga ou regime.
**Agora:** Sistema calcula automaticamente período de folga (quando aplicável) e bloqueia/libera dias conforme regime selecionado (12x36 ou 8h diárias).

### Como funciona?
1. Usuário seleciona: guarda + **regime (12x36 ou 8h)** + horário + primeiro dia
2. Sistema calcula conforme regime:
   - **12x36:** fim trabalho → fim folga (36h depois) → bloqueia dias ocupados 24h
   - **8h:** fim trabalho → libera imediatamente → permite dias consecutivos
3. Sistema analisa cada dia do mês:
   - Dia ocupado 24h? → Bloqueia (apenas 12x36)
   - Dia com horas livres? → Libera (ambos)
4. Calendário atualiza visualmente (verde/vermelho/amarelo)
5. Ao tentar adicionar dia com conflito → Aviso preventivo
6. Ao salvar → Validação final de todos os dias

### Regra de ouro:
**"Dia só bloqueia se guarda estiver ocupado 24 horas completas. Se sobrar nem 1 hora livre, dia fica disponível com aviso."**

### Padronizações:
- **Regimes:** 12x36 (plantão) ou 8h diárias (expediente)
- **Turnos:** apenas DIURNO e NOTURNO
- **Horários:** tipo TIME (HH:MM:SS)
- **Folga obrigatória:** apenas regime 12x36 (36 horas)
- **Cruzamento:** automático com escalas, férias, ausências, RET

### Diferença entre regimes:
```
12x36: Trabalha 1 dia → folga obrigatória → próximo dia disponível
8h: Trabalha 1 dia → disponível no dia seguinte → pode trabalhar consecutivo
```

### Objetivo final:
**Zero escalas conflitantes + respeito ao regime de trabalho + interface visual clara + conformidade trabalhista.**