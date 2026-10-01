using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;
public sealed class TaskRepository(TaskDbContext context) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default) => context.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyCollection<TaskItem>> GetAllAsync(TaskFilterRequest filter, CancellationToken ct = default)
    {
        IQueryable<TaskItem> query = context.Tasks.AsNoTracking();
        if (filter.Status.HasValue) query = query.Where(x => x.Status == filter.Status.Value);
        if (filter.DueDate.HasValue) { var start = filter.DueDate.Value.ToDateTime(TimeOnly.MinValue); var end = start.AddDays(1); query = query.Where(x => x.DueDate >= start && x.DueDate < end); }
        if (!string.IsNullOrWhiteSpace(filter.Search)) { var term = filter.Search.Trim().ToLower(); query = query.Where(x => x.Title.ToLower().Contains(term) || (x.Description != null && x.Description.ToLower().Contains(term))); }
        return await query.OrderBy(x => x.DueDate == null).ThenBy(x => x.DueDate).ThenBy(x => x.CreatedAt).ToArrayAsync(ct);
    }
    public Task AddAsync(TaskItem entity, CancellationToken ct = default) => context.Tasks.AddAsync(entity, ct).AsTask();
    public void Delete(TaskItem entity) => context.Tasks.Remove(entity);
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
