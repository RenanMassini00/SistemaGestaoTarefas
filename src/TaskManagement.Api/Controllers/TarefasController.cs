using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers;

/// <summary>
/// Disponibiliza operações para gerenciamento de tarefas.
/// </summary>
[ApiController]
[Route("api/tarefas")]
[Produces("application/json")]
public sealed class TarefasController(ITaskService service) : ControllerBase
{
    /// <summary>
    /// Cadastra uma nova tarefa.
    /// </summary>
    /// <param name="request">Dados da tarefa que será cadastrada.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>A tarefa cadastrada com seu código único.</returns>
    /// <response code="201">Tarefa cadastrada com sucesso.</response>
    /// <response code="400">Os dados enviados são inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(
    typeof(ValidationProblemDetails),
    StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TaskResponse>> Cadastrar(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await service.CreateAsync(
        request,
        cancellationToken);
 
        return Created(
        $"/api/tarefas/{resultado.Id}",
        resultado);
    }

    /// <summary>
    /// Lista todas as tarefas e permite aplicar filtros.
    /// </summary>
    /// <param name="filtro">Filtros opcionais de busca, status e data de vencimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Lista de tarefas encontradas.</returns>
    /// <response code="200">Consulta realizada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TaskResponse>>> Listar(
        [FromQuery] TaskFilterRequest filtro,
        CancellationToken cancellationToken)
    {
        var resultado = await service.GetAllAsync(filtro, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtém uma tarefa pelo código único.
    /// </summary>
    /// <param name="id">Código único da tarefa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>A tarefa encontrada.</returns>
    /// <response code="200">Tarefa encontrada.</response>
    /// <response code="404">Tarefa não encontrada.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> ObterPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var resultado = await service.GetByIdAsync(id, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Atualiza todos os dados de uma tarefa existente.
    /// </summary>
    /// <param name="id">Código único da tarefa.</param>
    /// <param name="request">Novos dados da tarefa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>A tarefa atualizada.</returns>
    /// <response code="200">Tarefa atualizada com sucesso.</response>
    /// <response code="400">Os dados enviados são inválidos.</response>
    /// <response code="404">Tarefa não encontrada.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Atualizar(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await service.UpdateAsync(id, request, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Exclui uma tarefa existente.
    /// </summary>
    /// <param name="id">Código único da tarefa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="204">Tarefa excluída com sucesso.</response>
    /// <response code="404">Tarefa não encontrada.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
