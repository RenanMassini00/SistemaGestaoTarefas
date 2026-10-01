using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public sealed class TaskItem
{
    private TaskItem() { }
    public TaskItem(string title, string? description, DateTime? dueDate, TaskItemStatus status, DateTime? now = null)
    {
        Id = Guid.NewGuid();
        UpdateFields(title, description, dueDate, status);
        CreatedAt = now ?? DateTime.UtcNow;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime? DueDate { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public void Update(string title, string? description, DateTime? dueDate, TaskItemStatus status, DateTime? now = null)
    {
        UpdateFields(title, description, dueDate, status);
        UpdatedAt = now ?? DateTime.UtcNow;
    }
    private void UpdateFields(string title, string? description, DateTime? dueDate, TaskItemStatus status)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        if (title.Trim().Length > 150) throw new ArgumentOutOfRangeException(nameof(title));
        if (description?.Length > 2000) throw new ArgumentOutOfRangeException(nameof(description));
        if (!Enum.IsDefined(status)) throw new ArgumentOutOfRangeException(nameof(status));
        Title = title.Trim(); Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(); DueDate = dueDate; Status = status;
    }
}
