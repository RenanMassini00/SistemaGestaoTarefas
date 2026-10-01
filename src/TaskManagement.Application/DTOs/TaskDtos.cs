using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs;

/// <summary>Dados necessários para cadastrar uma tarefa.</summary>
public sealed record CreateTaskRequest(
    [param: Required(ErrorMessage = "O título é obrigatório.")]
    [param: StringLength(150, MinimumLength = 3, ErrorMessage = "O título deve possuir entre 3 e 150 caracteres.")]
    string Title,
    [param: StringLength(2000, ErrorMessage = "A descrição deve possuir no máximo 2.000 caracteres.")]
    string? Description,
    DateTime? DueDate,
    TaskItemStatus Status);

/// <summary>Dados necessários para atualizar uma tarefa.</summary>
public sealed record UpdateTaskRequest(
    [param: Required(ErrorMessage = "O título é obrigatório.")]
    [param: StringLength(150, MinimumLength = 3, ErrorMessage = "O título deve possuir entre 3 e 150 caracteres.")]
    string Title,
    [param: StringLength(2000, ErrorMessage = "A descrição deve possuir no máximo 2.000 caracteres.")]
    string? Description,
    DateTime? DueDate,
    TaskItemStatus Status);

/// <summary>Representa os dados retornados de uma tarefa.</summary>
public sealed record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskItemStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

/// <summary>Filtros opcionais utilizados na consulta de tarefas.</summary>
public sealed record TaskFilterRequest(
    string? Search,
    TaskItemStatus? Status,
    DateOnly? DueDate);
