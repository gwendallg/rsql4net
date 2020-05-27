
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace RSql4Net.Models.Queries
{
    public class RSqlQueryModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                (context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(IRSqlQuery<>) &&
                 context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(RSqlQuery<>)))
            {
                return null;
            }

            var entityType = context.Metadata.ModelType.GetGenericArguments()[0];
            var modelBinderType = typeof(RSqlQueryModelBinder<>).MakeGenericType(entityType);
            return new BinderTypeModelBinder(modelBinderType);
        }
    }
}
