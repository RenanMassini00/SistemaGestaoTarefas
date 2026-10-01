using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Api.Serialization;

public sealed class TaskItemStatusModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = Nullable.GetUnderlyingType(context.Metadata.ModelType)
            ?? context.Metadata.ModelType;

        return modelType == typeof(TaskItemStatus)
            ? new BinderTypeModelBinder(typeof(TaskItemStatusModelBinder))
            : null;
    }
}
