using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers;
[ApiController]
[Route("api/tasks")]
public sealed class TasksController(ITaskService service) : ControllerBase
{
    /// <summary>Creates a task.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<ActionResult<TaskResponse>> Create(CreateTaskRequest request, CancellationToken ct)
    { var result = await service.CreateAsync(request, ct); return CreatedAtAction(nameof(GetById), new { id = result.Id }, result); }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskResponse>), 200)]
    public async Task<ActionResult<IReadOnlyCollection<TaskResponse>>> GetAll([FromQuery] TaskFilterRequest filter, CancellationToken ct) => Ok(await service.GetAllAsync(filter, ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), 200)] [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id, CancellationToken ct) => Ok(await service.GetByIdAsync(id, ct));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), 200)] [ProducesResponseType(typeof(ValidationProblemDetails), 400)] [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<TaskResponse>> Update(Guid id, UpdateTaskRequest request, CancellationToken ct) => Ok(await service.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)] [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) { await service.DeleteAsync(id, ct); return NoContent(); }
}
