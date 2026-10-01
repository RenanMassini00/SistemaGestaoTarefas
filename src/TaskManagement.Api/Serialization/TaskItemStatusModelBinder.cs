using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TaskManagement.Api.Serialization;

public sealed class TaskItemStatusModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        if (TaskItemStatusJsonConverter.TryParse(
                valueProviderResult.FirstValue,
                out var status))
        {
            bindingContext.Result = ModelBindingResult.Success(status);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                "Status inválido. Valores aceitos: Pendente, Em progresso, Concluída.");
        }

        return Task.CompletedTask;
    }
}
