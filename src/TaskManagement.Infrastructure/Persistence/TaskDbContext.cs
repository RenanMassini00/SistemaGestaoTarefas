using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence;
public sealed class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TaskItem>(); entity.HasKey(x => x.Id); entity.Property(x => x.Title).IsRequired().HasMaxLength(150); entity.Property(x => x.Description).HasMaxLength(2000);
    }
}
