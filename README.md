# Task Management API

API RESTful para o desafio de gestão de tarefas, construída com .NET 8, controllers, EF Core InMemory, Swagger, logging, ProblemDetails e testes xUnit.

## Arquitetura

Arquitetura em camadas pragmática: `Api` (HTTP), `Application` (casos de uso e DTOs), `Domain` (entidade e enum) e `Infrastructure` (EF Core e repositório). Essa separação aplica SOLID sem complexidade excessiva para o escopo.

## Pré-requisitos

- .NET SDK 8.0 ou superior, ou Docker

## Executar localmente

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/TaskManagement.Api
```

Swagger: `http://localhost:5098/swagger`

## Executar com Docker

```bash
docker compose up --build
```

Swagger: `http://localhost:8080/swagger`

## Endpoints

- `POST /api/tasks`
- `GET /api/tasks`
- `GET /api/tasks/{id}`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`

Filtros combináveis: `search`, `status` e `dueDate`. Exemplo: `/api/tasks?search=api&status=Pending&dueDate=2026-10-10`.

## Exemplo POST

```json
{
  "title": "Finalizar desafio técnico",
  "description": "Implementar API e testes",
  "dueDate": "2026-10-10T23:59:59Z",
  "status": "Pending"
}
```

Status aceitos: `Pending`, `InProgress`, `Completed`.

## Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Decisões técnicas

- `Guid` como código único.
- DTOs separam o contrato HTTP da entidade.
- Controllers pequenos delegam regras ao serviço.
- Repositório abstrai o EF Core.
- Middleware global padroniza erros com ProblemDetails.
- Logs estruturados com `ILogger`.
- `CancellationToken` em operações assíncronas.
- EF Core InMemory atende ao desafio, mas os dados são perdidos ao reiniciar e o provedor não é indicado para produção.

## Melhorias futuras

Paginação, autenticação, testes de integração, persistência SQL, versionamento da API e CI no GitHub Actions.
