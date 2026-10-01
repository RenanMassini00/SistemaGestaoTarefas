# API de Gestão de Tarefas

API REST para criar e gerenciar tarefas. O projeto usa .NET 8, ASP.NET Core Web API, Entity Framework Core InMemory, Swagger e testes automatizados com xUnit.

## Requisitos

- .NET SDK 8.0 ou superior.

## Executar

Na raiz do repositório:

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/TaskManagement.Api
```

O perfil HTTP local disponibiliza a API em `http://localhost:5098`. A documentação interativa do Swagger fica em `http://localhost:5098/swagger`.

## Endpoints

| Método | Rota | Resultado |
| --- | --- | --- |
| `POST` | `/api/tarefas` | Cria uma tarefa e retorna `201 Created` com seu identificador. |
| `GET` | `/api/tarefas` | Lista tarefas; aceita filtros opcionais. |
| `GET` | `/api/tarefas/{id}` | Obtém uma tarefa pelo identificador. |
| `PUT` | `/api/tarefas/{id}` | Atualiza título, descrição, vencimento e status. |
| `DELETE` | `/api/tarefas/{id}` | Exclui uma tarefa e retorna `204 No Content`. |

Os filtros de listagem podem ser combinados:

- `search`: procura texto no título ou na descrição;
- `status`: filtra por `Pendente`, `Em progresso` ou `Concluída`;
- `dueDate`: filtra por dia no formato `yyyy-MM-dd`.

Exemplo: `/api/tarefas?search=api&status=Em%20progresso&dueDate=2026-10-10`.

## Exemplo de criação

```http
POST /api/tarefas
Content-Type: application/json
```

```json
{
  "title": "Finalizar desafio técnico",
  "description": "Implementar e testar a API",
  "dueDate": "2026-10-10T23:59:59Z",
  "status": "Pendente"
}
```

O título é obrigatório e deve ter entre 3 e 150 caracteres. A descrição é opcional e aceita até 2.000 caracteres. Data de vencimento também é opcional. Os nomes de status em português são os valores documentados e retornados pela API; os nomes ingleses anteriores (`Pending`, `InProgress` e `Completed`) continuam aceitos em requisições para compatibilidade.

As respostas seguem os códigos HTTP apropriados:

- `200 OK`: consulta ou atualização concluída;
- `201 Created`: tarefa criada;
- `204 No Content`: tarefa excluída;
- `400 Bad Request`: JSON, status ou dados inválidos;
- `404 Not Found`: identificador de tarefa inexistente;
- `500 Internal Server Error`: falha inesperada, com identificador de rastreamento na resposta.

## Organização e arquitetura

O código é dividido em projetos por responsabilidade:

```text
src/
  TaskManagement.Api/             HTTP, controllers, Swagger e middleware
  TaskManagement.Application/     serviços, DTOs, interfaces e exceções de aplicação
  TaskManagement.Domain/          entidade TaskItem e enum de status
  TaskManagement.Infrastructure/  EF Core, DbContext e repositório
tests/
  TaskManagement.UnitTests/       testes xUnit de serviço e domínio
```

### Responsabilidades e fluxo de uma requisição

1. **API / Controller** recebe e valida a requisição HTTP, escolhe o status e produz a resposta. `TarefasController` converte operações HTTP em chamadas ao serviço.
2. **Application / Service** implementa os casos de uso. `TaskService` cria, consulta, atualiza e exclui tarefas; converte entidades em `TaskResponse`; depende da abstração `ITaskRepository`, não do Entity Framework.
3. **Domain** contém as regras centrais. `TaskItem` garante título não vazio, limites de tamanho, status válido, normalização de texto e datas de criação/atualização.
4. **Infrastructure / Repository** implementa `ITaskRepository` com `TaskDbContext` e Entity Framework Core. A consulta de listagem aplica os filtros e ordena os resultados.
5. **Dependency injection** configura `ITaskService`, `ITaskRepository` e `TaskDbContext` no início da aplicação, ligando as abstrações às implementações concretas.

Exemplo do POST: JSON → model binding e validação → `TarefasController` → `TaskService` → `TaskItem` → `TaskRepository`/`TaskDbContext` → resposta `201 Created` com a tarefa criada.

### Decisões alinhadas ao desafio

- DTOs separam o contrato HTTP da entidade de domínio e evitam expor a persistência diretamente.
- Interfaces e injeção de dependência reduzem o acoplamento entre serviço e repositório e facilitam testes isolados.
- O serviço e o repositório preservam a separação Controller/Service/Repository solicitada.
- O middleware global transforma exceções conhecidas em `400` ou `404`, registra falhas inesperadas e devolve `ProblemDetails` com `traceId`.
- Swagger documenta e permite experimentar os endpoints e os valores de status.
- xUnit, Moq e FluentAssertions são usados para testar regras do serviço e da entidade.
- `CancellationToken` é propagado pelas operações assíncronas.

## Testes

```bash
dotnet test
```

Os testes cobrem criação, consulta, atualização, exclusão, mapeamento das respostas e validações importantes da entidade. Para coletar cobertura, se o coletor estiver disponível:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Persistência e escopo

O desafio pede EF Core com persistência em memória, por isso a API usa `UseInMemoryDatabase`. Os dados ficam disponíveis enquanto a aplicação está em execução e são perdidos quando o processo reinicia; esse provedor não é indicado para persistência de produção. A aplicação atualmente oferece a API e a documentação Swagger, não uma interface web de usuário.
