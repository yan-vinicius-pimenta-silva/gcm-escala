# Guia de Execucao - Sistema de Gestao de Escalas GCM

## Pre-requisitos

- **.NET 9 SDK** (ou superior)
- **Node.js 18+** (com npm)
- **dotnet-ef** tool (para migrations):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## 1. Backend (API)

### Restaurar dependencias e compilar

```bash
cd backend/src/EscalaGcm.Api
dotnet restore
dotnet build
```

### Executar a API

```bash
cd backend/src/EscalaGcm.Api
dotnet run --urls http://localhost:5062
```

> **Importante:** O frontend esta configurado para se conectar em `http://localhost:5062/api`.
> Use a flag `--urls` para garantir que a API suba nesta porta.

Na primeira execucao, o sistema automaticamente:
- Cria o banco SQLite (`escala_gcm.db`)
- Executa as migrations pendentes
- Popula os dados iniciais (usuario admin + 22 setores)

### Verificar se a API esta rodando

Acesse no navegador: http://localhost:5062/swagger

### Swagger

A documentacao interativa da API esta disponivel em `/swagger` quando rodando em modo Development.

## 2. Frontend (React)

### Instalar dependencias

```bash
cd frontend
npm install
```

### Executar em modo desenvolvimento

```bash
cd frontend
npm run dev
```

O frontend sobe em: http://localhost:5173

### Build de producao

```bash
cd frontend
npm run build
```

Os arquivos ficam em `frontend/dist/`.

## 3. Login

Acesse http://localhost:5173 no navegador.

**Credenciais padroes:**
- Usuario: `admin`
- Senha: `admin123`

## 4. Ordem de Uso Recomendada

1. **Posicoes** - Cadastrar posicoes dos guardas (ex: Inspetor, Sub-inspetor, GCM)
2. **Turnos** - Cadastrar turnos de trabalho (ex: Diurno, Noturno)
3. **Horarios** - Cadastrar faixas horarias (ex: 07:00-19:00, 19:00-07:00)
4. **Guardas** - Cadastrar guardas vinculando a uma posicao
5. **Viaturas** - Cadastrar viaturas disponiveis
6. **Equipes** - Montar equipes de 2 a 4 guardas
7. **Ferias / Ausencias** - Registrar periodos de ferias e ausencias
8. **Escalas** - Montar a escala operacional por setor/quinzena
9. **Relatorios** - Gerar relatorios e exportar para Excel

## 5. Modulo Escalas - Fluxo de Trabalho

1. Selecione **Setor**, **Mes**, **Ano** e **Quinzena**
2. Clique em **Carregar** (cria ou carrega a escala existente)
3. No formulario a esquerda:
   - Selecione Turno e Horario
   - Selecione os dias da quinzena
   - Preencha as alocacoes conforme o tipo do setor
   - Clique em **Adicionar a Escala**
4. Os lancamentos aparecem na tabela a direita
5. Quando finalizado, clique em **Publicar** (bloqueia edicao)
6. Para encerrar definitivamente, clique em **Fechar**

### Regras por tipo de setor

| Tipo | Alocacao necessaria |
|------|-------------------|
| Padrao | 1 ou mais guardas (Integrante) |
| Central de Comunicacoes | 1 equipe |
| Radio Patrulha | Motorista + Encarregado + Apoio + Viatura (opcional) |
| Divisao Rural | Motorista + Encarregado |
| ROMU | Motorista + Encarregado + Integrante (opcional) |
| Ronda Comercio | Motorista + Encarregado |

## 6. Modulo Relatorios

Tipos disponiveis:
1. **Escala Mensal por Setor** - Todos os lancamentos do mes por setor
2. **Guardas Escalados** - Guardas com plantoes no mes e contagem
3. **Guardas Nao Escalados** - Guardas ativos sem nenhum plantao no mes
4. **Ferias** - Ferias que coincidem com o mes selecionado
5. **Ausencias** - Ausencias que coincidem com o mes selecionado
6. **Individual do Guarda** - Agenda completa de um guarda no mes

Todos os relatorios podem ser exportados para **Excel (.xlsx)**.

## 7. Estrutura do Projeto

```
escala_gcm_app/
├── docs/                          # Documentacao do projeto
├── backend/
│   ├── EscalaGcm.sln
│   └── src/
│       ├── EscalaGcm.Api/        # Controllers, Program.cs (porta 5062)
│       ├── EscalaGcm.Application/ # DTOs, interfaces de servico
│       ├── EscalaGcm.Domain/     # Entidades, enums (sem dependencias)
│       └── EscalaGcm.Infrastructure/ # EF Core, servicos, repositorios
├── frontend/
│   └── src/
│       ├── api/                   # Cliente HTTP (axios)
│       ├── components/            # Componentes compartilhados
│       ├── features/              # Modulos (setores, guardas, escalas, etc.)
│       ├── contexts/              # AuthContext
│       ├── layouts/               # Layout principal com sidebar
│       ├── routes/                # Rotas protegidas
│       └── types/                 # Interfaces TypeScript
└── execution.md                   # Este arquivo
```

## 8. Banco de Dados

- **SQLite** em modo desenvolvimento (arquivo `escala_gcm.db`)
- Migrations sao aplicadas automaticamente ao iniciar a API
- Para recriar o banco do zero, delete o arquivo `escala_gcm.db` e reinicie a API

### Gerar nova migration (apos alterar entidades)

```bash
cd backend/src/EscalaGcm.Api
dotnet ef migrations add NomeDaMigration --project ../EscalaGcm.Infrastructure
```

## 9. Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| Frontend | React 19, TypeScript 5.9, Vite 7, MUI 7, TanStack Query 5 |
| Backend | ASP.NET Core 9, Entity Framework Core 9, SQLite |
| Auth | JWT Bearer + BCrypt |
| Export | ClosedXML (Excel) |
| Validacao | Zod (frontend), FluentValidation patterns (backend) |
