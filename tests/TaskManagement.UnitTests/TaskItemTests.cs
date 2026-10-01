using FluentAssertions;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Xunit;

namespace TaskManagement.UnitTests;

public sealed class TaskItemTests
{
    [Fact]
    public void Constructor_ShouldTrimTitleAndDescription_AndSetCreationDate()
    {
        var createdAt = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);

        var task = new TaskItem(
            "  Prepare challenge  ",
            "  API implementation  ",
            null,
            TaskItemStatus.Pending,
            createdAt);

        task.Title.Should().Be("Prepare challenge");
        task.Description.Should().Be("API implementation");
        task.CreatedAt.Should().Be(createdAt);
        task.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldRejectBlankTitle(string title)
    {
        var action = () => new TaskItem(title, null, null, TaskItemStatus.Pending);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldRejectTitleOver150Characters()
    {
        var action = () => new TaskItem(
            new string('a', 151),
            null,
            null,
            TaskItemStatus.Pending);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_ShouldRejectDescriptionOver2000Characters()
    {
        var action = () => new TaskItem(
            "Valid title",
            new string('a', 2001),
            null,
            TaskItemStatus.Pending);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_ShouldRejectUndefinedStatus()
    {
        var action = () => new TaskItem(
            "Valid title",
            null,
            null,
            (TaskItemStatus)0);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
