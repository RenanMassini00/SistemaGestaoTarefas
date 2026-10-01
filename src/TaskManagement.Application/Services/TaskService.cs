using Microsoft.Extensions.Logging;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services;
public sealed class TaskService(ITaskRepository repository, ILogger<TaskService> logger) : ITaskService
{
    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        var entity = new TaskItem(request.Title, request.Description, request.DueDate, request.Status);
        await repository.AddAsync(entity, ct); await repository.SaveChangesAsync(ct);
        logger.LogInformation("Task {TaskId} created", entity.Id); return Map(entity);
    }
    public async Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(TaskFilterRequest filter, CancellationToken ct = default)
        => (await repository.GetAllAsync(filter, ct)).Select(Map).ToArray();
    public async Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken ct = default) => Map(await Find(id, ct));
    public async Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default)
    {
        var entity = await Find(id, ct); entity.Update(request.Title, request.Description, request.DueDate, request.Status);
        await repository.SaveChangesAsync(ct); logger.LogInformation("Task {TaskId} updated", id); return Map(entity);
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await Find(id, ct); repository.Delete(entity); await repository.SaveChangesAsync(ct); logger.LogInformation("Task {TaskId} deleted", id);
    }
    private async Task<TaskItem> Find(Guid id, CancellationToken ct) => await repository.GetByIdAsync(id, ct) ?? throw new NotFoundException($"Task '{id}' was not found.");
    private static TaskResponse Map(TaskItem x) => new(x.Id, x.Title, x.Description, x.DueDate, x.Status, x.CreatedAt, x.UpdatedAt);
}
