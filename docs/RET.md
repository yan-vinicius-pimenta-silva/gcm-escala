# REGRA RET - REGIME ESPECIAL DE TRABALHO

## DEFINIÇÃO
RET é uma jornada intermediária de **8 horas fixas** entre escalas normais, utilizada para cobrir necessidades pontuais de efetivo. 
Nem todos os guardas fazem RET no mês (são ~200 guardas, RET é seletivo).

---

## TIPOS DE RET

### 1. RET MENSAL
- Limite: **1 por guarda por mês**
- Disponível: em qualquer mês
- Opcional: guarda pode ou não fazer

### 2. RET DE EVENTO
- Limite: **1 adicional por guarda por mês**
- Disponível: **somente em meses com evento cadastrado**
- Opcional: guarda pode ou não fazer
- Exemplo: Carnaval, festividades municipais

---

## RESTRIÇÕES GERAIS

### ❌ Bloqueios Absolutos
1. **Máximo 2 RETs por guarda por mês** (1 mensal + 1 evento)
2. **Proibido 2 RETs na mesma semana** (domingo a sábado)
3. **RET sempre tem duração de 8 horas** (não pode variar)

### Exemplo de bloqueio semanal:
```
Semana 2 (16-22/02):
  - Dia 17/02: RET MENSAL ✅
  - Dia 19/02: RET EVENTO ❌ BLOQUEADO (mesma semana)
```

---

## REGRA DOS DESCANSOS (12 HORAS)

### Princípio Base
**RET exige 12 horas de descanso antes e depois, MAS apenas quando há escala adjacente.**

### Cenários Possíveis

#### CENÁRIO A: Escala → RET → Escala (32 horas totais)
```
ESCALA NORMAL encerra
    ↓ [12h descanso] ← OBRIGATÓRIO
RET (8h)
    ↓ [12h descanso] ← OBRIGATÓRIO
ESCALA NORMAL inicia

TOTAL: 12h + 8h + 12h = 32 horas
```
**Validação:** Intervalo total entre escalas ≥ 32h


#### CENÁRIO B: Escala → RET (sem escala depois)
```
ESCALA NORMAL encerra
    ↓ [12h descanso] ← OBRIGATÓRIO
RET (8h)
    (fim do período)
```
**Validação:** Apenas descanso antes ≥ 12h


#### CENÁRIO C: RET → Escala (sem escala antes)
```
(início do período)
RET (8h)
    ↓ [12h descanso] ← OBRIGATÓRIO
ESCALA NORMAL inicia
```
**Validação:** Apenas descanso depois ≥ 12h
**Motivo:** Guarda estava disponível (sem escala anterior)


#### CENÁRIO D: RET Isolado (sem escalas)
```
(sem escala antes)
RET (8h)
(sem escala depois)
```
**Validação:** Nenhuma validação de descanso
**Motivo:** Guarda disponível, sem escalas no período


---

## FLUXO DE VALIDAÇÃO

### Ao criar/editar RET do guarda:

1. ✅ **Duração é 8h?**
   - Se não → BLOQUEAR

2. ✅ **Buscar escala anterior do guarda**
   - Se existe E término + 12h > início RET → BLOQUEAR
   - Se não existe → PERMITIR (entrada direta)

3. ✅ **Buscar próxima escala do guarda**
   - Se existe E fim RET + 12h > início escala → BLOQUEAR
   - Se não existe → PERMITIR (RET isolado)

4. ✅ **Se há escala antes E depois**
   - Validar: intervalo total ≥ 32h
   - Se não → BLOQUEAR

5. ✅ **Contar RETs do guarda no mês**
   - RET MENSAL: já tem 1? → BLOQUEAR
   - RET EVENTO: já tem 1? → BLOQUEAR

6. ✅ **Verificar semana do RET**
   - Já existe outro RET na mesma semana? → BLOQUEAR

7. ✅ **Se RET DE EVENTO**
   - Existe evento cadastrado no mês? Se não → BLOQUEAR

---

## EVENTOS ESPECIAIS

### Cadastro de Evento
```
Campos obrigatórios:
- Nome do evento (ex: "Carnaval 2026")
- Data início (ex: 14/02/2026)
- Data fim (ex: 17/02/2026)
- Semana do evento (calculada automaticamente)
```

### Efeito no RET
- Habilita opção "RET DE EVENTO" no formulário
- Guarda pode fazer até 2 RETs no mês (mensal + evento)
- Regras de descanso (12h) continuam aplicadas
- Bloqueio de mesma semana continua aplicado

---

## EXEMPLOS PRÁTICOS

### ✅ CASO VÁLIDO 1: RET entre escalas
```
10/02 18:00 - Escala encerra
11/02 06:00 - RET inicia (12h depois) ✅
11/02 14:00 - RET encerra
12/02 02:00 - Escala inicia (12h depois) ✅
```

### ✅ CASO VÁLIDO 2: Guarda disponível
```
(sem escala antes)
11/02 06:00 - RET inicia ✅ (entrada direta)
11/02 14:00 - RET encerra
12/02 02:00 - Escala inicia (12h depois) ✅
```

### ❌ CASO INVÁLIDO 1: Descanso insuficiente
```
10/02 18:00 - Escala encerra
11/02 02:00 - RET inicia ❌ (apenas 8h de descanso)
```

### ❌ CASO INVÁLIDO 2: Mesma semana
```
Semana 16-22/02:
  17/02 - RET MENSAL ✅
  19/02 - RET EVENTO ❌ (mesma semana bloqueada)
```

### ✅ CASO VÁLIDO 3: Semanas diferentes
```
Semana 09-15/02:
  14/02 - RET MENSAL ✅

Semana 16-22/02:
  18/02 - RET EVENTO ✅ (semana diferente)
```

---

## RESUMO EXECUTIVO

**O que é RET:** Jornada especial de 8h entre escalas normais

**Quantos por mês:**
- Sem evento: até 1 RET
- Com evento: até 2 RETs (1 mensal + 1 evento)

**Regra principal:**
- COM escala antes: precisa 12h de descanso antes do RET
- COM escala depois: precisa 12h de descanso depois do RET
- COM ambas: total de 32h entre escalas (12 + 8 + 12)
- SEM escala adjacente: não valida descanso daquele lado

**Bloqueios:**
- 2 RETs na mesma semana
- Mais de 1 RET mensal no mês
- Mais de 1 RET evento no mês
- Duração diferente de 8h
- Descanso < 12h quando há escala adjacente