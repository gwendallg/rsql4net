using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace RSql4Net.Models.Paging
{
    /// <summary>
    ///     Pageable model binder provider.
    /// </summary>
    public class RSqlPageableModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        ///     Gets the binder.
        /// </summary>
        /// <returns>The binder.</returns>
        /// <param name="context">Context.</param>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                (context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(IRSqlPageable<>) &&
                 context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(RSqlPageable<>)))
            {
                return null;
            }
            var entityType = context.Metadata.ModelType.GetGenericArguments()[0];
            var modelBinderType = typeof(RSqlPageableModelBinder<>).MakeGenericType(entityType);
            return new BinderTypeModelBinder(modelBinderType);
        }
    }
}
