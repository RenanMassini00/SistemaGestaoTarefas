using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Xunit;

namespace TaskManagement.UnitTests;

public sealed class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repository = new();
    private TaskService Service => new(_repository.Object, NullLogger<TaskService>.Instance);

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnTask()
    {
        var request = new CreateTaskRequest(
            "Implement API",
            "Challenge",
            DateTime.UtcNow.AddDays(2),
            TaskItemStatus.Pending);

        var result = await Service.CreateAsync(request);

        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be(request.Title);
        _repository.Verify(
            repository => repository.AddAsync(
                It.IsAny<TaskItem>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _repository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenTaskDoesNotExist()
    {
        _repository
            .Setup(repository => repository.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var action = () => Service.GetByIdAsync(Guid.NewGuid());

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateFields()
    {
        var entity = new TaskItem("Old title", null, null, TaskItemStatus.Pending);
        _repository
            .Setup(repository => repository.GetByIdAsync(
                entity.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await Service.UpdateAsync(
            entity.Id,
            new UpdateTaskRequest(
                "New title",
                "Description",
                null,
                TaskItemStatus.Completed));

        result.Title.Should().Be("New title");
        result.Status.Should().Be(TaskItemStatus.Completed);
        result.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteExistingTask()
    {
        var entity = new TaskItem("Task title", null, null, TaskItemStatus.Pending);
        _repository
            .Setup(repository => repository.GetByIdAsync(
                entity.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await Service.DeleteAsync(entity.Id);

        _repository.Verify(repository => repository.Delete(entity), Times.Once);
        _repository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedTasks()
    {
        var tasks = new[]
        {
            new TaskItem("First task", null, null, TaskItemStatus.Pending),
            new TaskItem("Second task", null, null, TaskItemStatus.Completed)
        };
        _repository
            .Setup(repository => repository.GetAllAsync(
                It.IsAny<TaskFilterRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var result = await Service.GetAllAsync(new TaskFilterRequest(null, null, null));

        result.Should().HaveCount(2);
    }
}
