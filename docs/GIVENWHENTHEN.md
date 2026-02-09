## Critérios de Aceite Completos (Given / When / Then)

Este documento detalha os critérios de aceite funcionais do sistema de escalas, com foco nas regras críticas e no cruzamento de dados.

---

# 1. Módulo Setores

### Cadastro de setor

**Given** o usuário está na tela de setores
**When** informa um nome válido e salva
**Then** o setor deve ser criado e aparecer na lista

### Bloqueio de duplicidade

**Given** já existe um setor com o mesmo nome
**When** o usuário tenta cadastrar novamente
**Then** o sistema deve impedir o cadastro e informar duplicidade

### Exclusão bloqueada por uso

**Given** um setor possui escalas vinculadas
**When** o usuário tenta excluir
**Then** o sistema deve bloquear a exclusão

---

# 2. Módulo Guardas

### Cadastro válido

**Given** o usuário informa nome e posição
**When** salva
**Then** o guarda deve ser registrado

### Exclusão bloqueada

**Given** o guarda possui escalas registradas
**When** o usuário tenta excluir
**Then** o sistema deve impedir a exclusão

---

# 3. Módulo Equipes

### Regra de quantidade mínima

**Given** uma equipe possui menos de 2 membros
**When** o usuário tenta salvar
**Then** o sistema deve impedir

### Regra de quantidade máxima

**Given** uma equipe possui mais de 4 membros
**When** o usuário tenta salvar
**Then** o sistema deve impedir

---

# 4. Módulo Férias

### Cadastro válido

**Given** o usuário seleciona um guarda e datas válidas
**When** salva
**Then** o registro deve ser criado

### Bloqueio por sobreposição

**Given** já existe férias no período
**When** o usuário tenta cadastrar outro período que sobrepõe
**Then** o sistema deve bloquear

---

# 5. Módulo Ausências

### Cadastro válido

**Given** o usuário informa guarda, datas e motivo
**When** salva
**Then** o registro deve ser criado

### Bloqueio por sobreposição

**Given** já existe ausência no período
**When** o usuário tenta cadastrar outra sobreposta
**Then** o sistema deve bloquear

---

# 6. Módulo Escalas (Regras Gerais)

### Cadastro básico

**Given** o usuário seleciona setor, turno, horário, mês, ano e dias
**When** salva
**Then** o sistema deve criar a escala

---

### Bloqueio por férias

**Given** o guarda está de férias em um dos dias
**When** o usuário tenta salvar a escala
**Then** o sistema deve impedir o salvamento

---

### Bloqueio por ausência

**Given** o guarda possui ausência registrada
**When** tenta escalá-lo no período
**Then** o sistema deve bloquear

---

### Bloqueio por conflito de escala

**Given** o guarda já está escalado no mesmo dia e horário
**When** tenta escalá-lo novamente
**Then** o sistema deve impedir

---

### Bloqueio por duplicidade

**Given** existe escala com os mesmos parâmetros
**When** tenta salvar novamente
**Then** o sistema deve impedir

---

# 7. Regras Especiais por Setor

---

## 7.1 Central de Comunicações

### Escala por equipe

**Given** o setor é Central de Comunicações
**When** o usuário tenta salvar sem equipe
**Then** o sistema deve impedir

---

## 7.2 Rádio Patrulha

### Obrigatoriedade de funções

**Given** o setor é Rádio Patrulha
**When** não existe motorista
**Then** o sistema deve impedir salvar

**Given** o setor é Rádio Patrulha
**When** não existe encarregado
**Then** o sistema deve impedir salvar

**Given** o setor é Rádio Patrulha
**When** não existe equipe de apoio
**Then** o sistema deve impedir salvar

---

## 7.3 Divisão Rural

**Given** o setor é Divisão Rural
**When** não existe motorista ou encarregado
**Then** o sistema deve impedir salvar

---

## 7.4 ROMU

**Given** o setor é ROMU
**When** não existe motorista ou encarregado
**Then** o sistema deve impedir salvar

**Given** o setor é ROMU
**When** existe terceiro integrante
**Then** o sistema deve permitir

---

## 7.5 Ronda Comércio

**Given** o setor é Ronda Comércio
**When** não existem motorista e encarregado
**Then** o sistema deve impedir salvar

---

# 8. Regras de Quinzena

### Primeira quinzena

**Given** a escala é da primeira quinzena
**When** o usuário seleciona dias maiores que 15
**Then** o sistema deve impedir

### Segunda quinzena

**Given** a escala é da segunda quinzena
**When** o usuário seleciona dias menores que 16
**Then** o sistema deve impedir

---

# 9. Relatórios

### Relatório mensal

**Given** o usuário informa mês e ano
**When** gera relatório
**Then** o sistema deve exibir dados corretos

---

### Relatório individual

**Given** o usuário seleciona um guarda
**When** gera relatório
**Then** o sistema deve mostrar apenas registros do guarda

---

# 10. Performance e Confiabilidade

### Tempo de resposta

**Given** operação normal de cadastro
**When** salva
**Then** deve responder em até 2 segundos

---

### Integridade transacional

**Given** ocorre falha durante gravação
**When** a operação é interrompida
**Then** nenhum registro parcial deve permanecer

---

# 11. Segurança

### Controle de acesso

**Given** usuário sem permissão
**When** tenta excluir escala
**Then** o sistema deve bloquear

---

# 12. Critérios de Aceite para Entrega do Sistema

O sistema será considerado pronto quando:

* todas as validações estiverem funcionando
* cruzamento de dados estiver bloqueando inconsistências
* regras especiais por setor estiverem aplicadas
* relatórios gerarem corretamente
* seja possível montar uma escala mensal completa sem erro

---

# 13. Requisito Crítico (o mais importante)

O sistema deve sempre garantir:

1. Nenhum guarda escalado em férias
2. Nenhum guarda em dois lugares no mesmo horário
3. Nenhuma equipe inválida
4. Nenhuma escala fora da quinzena
5. Nenhum setor especial sem suas regras

Esses cinco pontos são obrigatórios para o sistema funcionar corretamente.

---

