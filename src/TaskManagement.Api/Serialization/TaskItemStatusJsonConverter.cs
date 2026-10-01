using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Api.Serialization;

public sealed class TaskItemStatusJsonConverter : JsonConverter<TaskItemStatus>
{
    public static bool TryParse(string? value, out TaskItemStatus status)
    {
        switch (value?.Trim().ToUpperInvariant())
        {
            case "PENDENTE":
            case "PENDING":
                status = TaskItemStatus.Pending;
                return true;
            case "EM PROGRESSO":
            case "INPROGRESS":
            case "IN PROGRESS":
                status = TaskItemStatus.InProgress;
                return true;
            case "CONCLUÍDA":
            case "CONCLUIDA":
            case "COMPLETED":
                status = TaskItemStatus.Completed;
                return true;
            default:
                status = default;
                return false;
        }
    }

    public override TaskItemStatus Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var value))
        {
            var numericStatus = (TaskItemStatus)value;
            if (Enum.IsDefined(numericStatus))
            {
                return numericStatus;
            }
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var status = reader.GetString();
            if (TryParse(status, out var parsedStatus))
            {
                return parsedStatus;
            }

            throw new JsonException($"Status de tarefa inválido: '{status}'.");
        }

        throw new JsonException("O status da tarefa deve ser um dos valores reconhecidos.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        TaskItemStatus value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            TaskItemStatus.Pending => "Pendente",
            TaskItemStatus.InProgress => "Em progresso",
            TaskItemStatus.Completed => "Concluída",
            _ => throw new JsonException($"Status de tarefa inválido: '{value}'.")
        });
    }
}
