using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyCollection<TaskItem>> GetAllAsync(TaskFilterRequest filter, CancellationToken ct = default);
    Task AddAsync(TaskItem entity, CancellationToken ct = default);
    void Delete(TaskItem entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
public interface ITaskService
{
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(TaskFilterRequest filter, CancellationToken ct = default);
    Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
