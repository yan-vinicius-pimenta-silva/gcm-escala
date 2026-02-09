## PRD — Sistema de Gestão de Escalas Mensais (com montagem quinzenal)

---

# 1. Visão do Produto

## 1.1 Nome do sistema

Sistema de Gestão de Escalas Operacionais.

## 1.2 Objetivo

Permitir o cadastro de dados operacionais e a montagem de escalas mensais organizadas por quinzena, garantindo consistência através do cruzamento automático de dados (escala, férias, ausências e conflitos).

## 1.3 Problema que o sistema resolve

Atualmente a montagem de escalas exige controle manual e verificação visual, o que pode gerar:

* duplicidade de guardas
* escalas em período de férias
* conflitos de horários
* dificuldade para gerar relatórios

O sistema elimina esses problemas com validações automáticas.

---

# 2. Escopo do Produto

O sistema será dividido em abas:

1. Setores
2. Posições
3. Turnos
4. Horários
5. Equipes
6. Viaturas
7. Guardas
8. Escalas
9. Férias
10. Ausências
11. Relatórios

---

# 3. Usuários do Sistema

## 3.1 Administrador

Pode:

* cadastrar e excluir dados
* montar escalas
* editar escalas
* gerar relatórios

## 3.2 Operador

Pode:

* montar escalas
* visualizar relatórios
* cadastrar férias e ausências

## 3.3 Consulta

Pode:

* visualizar escalas
* visualizar relatórios

---

# 4. Requisitos Funcionais

---

# 4.1 Módulo Setores

O sistema deve permitir:

* cadastrar setor
* visualizar setores
* editar setor
* excluir setor

O sistema deve iniciar com a lista de setores predefinida.

Campos:

* Nome do setor
* Status (ativo/inativo)

Critérios de aceite:

* Não permitir setores duplicados
* Não permitir excluir setor que possua escalas vinculadas

---

# 4.2 Módulo Posições

Permitir:

* cadastrar posição
* editar posição
* excluir posição
* visualizar posições

Campos:

* Nome da posição

Critérios:

* Não permitir excluir posição em uso

---

# 4.3 Módulo Turnos

Permitir:

* cadastrar turno
* editar turno
* excluir turno
* visualizar turnos

Campos:

* Nome do turno

---

# 4.4 Módulo Horários

Permitir:

* cadastrar horário
* editar horário
* excluir horário
* visualizar horários

Campos:

* Hora início
* Hora fim
* Descrição

Critérios:

* Não permitir horários inválidos

---

# 4.5 Módulo Equipes

Permitir:

* cadastrar equipe
* editar equipe
* excluir equipe
* visualizar equipes

Campos:

* Nome da equipe

Regras:

* Equipe deve possuir entre 2 e 4 guardas

---

# 4.6 Módulo Viaturas

Permitir:

* cadastrar viatura
* editar viatura
* excluir viatura
* visualizar viaturas

Campos:

* Identificação da viatura

---

# 4.7 Módulo Guardas

Permitir:

* cadastrar guarda
* editar guarda
* excluir guarda
* visualizar guardas

Campos:

* Nome
* Telefone
* Posição

Critérios:

* Não permitir excluir guarda com registros históricos

---

# 4.8 Módulo Escalas

Permitir:

* cadastrar escala
* editar escala
* excluir escala
* visualizar escala

Fluxo de criação:

1 Selecionar setor
2 Selecionar guardas ou equipe
3 Selecionar turno
4 Selecionar horário
5 Selecionar mês
6 Selecionar ano
7 Selecionar dias

O sistema deve automaticamente:

* validar conflitos
* validar férias
* validar ausências

---

# 4.9 Regras Especiais por Setor

## Central de Comunicações

Escala deve permitir selecionar equipe em vez de guardas.

## Rádio Patrulha

Obrigatório:

* motorista
* encarregado
* equipe de apoio

## Divisão Rural

Obrigatório:

* motorista
* encarregado

## ROMU

Obrigatório:

* motorista
* encarregado
* terceiro integrante opcional

## Ronda Comércio

Obrigatório:

* motorista
* encarregado

---

# 4.10 Módulo Férias

Permitir:

* cadastrar férias
* editar férias
* excluir férias
* visualizar férias

Campos:

* Guarda
* Data início
* Data fim

Regras:

* Não permitir sobreposição de férias

---

# 4.11 Módulo Ausências

Permitir:

* cadastrar ausência
* editar ausência
* excluir ausência
* visualizar ausências

Campos:

* Guarda
* Data início
* Data fim
* Motivo
* Observação

Motivos:

* Atestado médico
* Doação de sangue
* Afastamento judicial
* Falta sem motivo
* Outros

---

# 4.12 Módulo Relatórios

Permitir gerar:

1 Relatório de escala mensal por setor
2 Relatório de guardas escalados
3 Relatório de guardas não escalados
4 Relatório de férias
5 Relatório de ausências
6 Relatório individual

Filtros:

* mês
* ano
* guarda (opcional)
* setor (opcional)

---

# 5. Regras de Negócio

O sistema deve impedir:

* guarda escalado em dois lugares no mesmo horário
* guarda escalado durante férias
* guarda escalado durante ausência
* duplicidade de escala
* equipes com menos de 2 integrantes
* equipes com mais de 4 integrantes

---

# 6. Requisitos Não Funcionais

O sistema deve:

* responder em menos de 2 segundos em operações comuns
* manter histórico de alterações
* possuir autenticação
* suportar no mínimo 50 usuários simultâneos
* funcionar em navegador moderno

---

# 7. Critérios de Aceite Gerais

O sistema será considerado pronto quando:

* todos os módulos estiverem operacionais
* regras de conflito estiverem funcionando
* relatórios gerarem dados corretos
* seja possível montar escala mensal completa

---

# 8. Roadmap de Desenvolvimento

Fase 1:
Cadastro base:

* setores
* guardas
* posições
* turnos
* horários

Fase 2:

* equipes
* viaturas
* férias
* ausências

Fase 3:

* escalas
* regras especiais

Fase 4:

* relatórios
* melhorias de performance

---

# 9. Riscos

Principais riscos:

* regras de conflito mal definidas
* inconsistência de dados históricos
* performance em relatórios grandes

Mitigação:

* validações no backend
* índices no banco
* testes automatizados

---

Se quiser, o próximo passo ideal é o **Modelo de Banco de Dados (ERD profissional)**. Ele é a base que define se o sistema vai funcionar bem ou não, principalmente por causa das regras especiais de escala.
