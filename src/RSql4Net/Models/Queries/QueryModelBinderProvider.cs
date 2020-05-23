using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RSql4Net.Configurations;

namespace RSql4Net.Models.Queries
{
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        private readonly Settings _settings;

        public QueryModelBinderProvider(Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                (context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(IQuery<>) &&
                 context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(Query<>)))
            {
                return null;
            }

            var entityType = context.Metadata.ModelType.GetGenericArguments()[0];
            var modelBinderType = typeof(QueryModelBinder<>).MakeGenericType(entityType);
            return (IModelBinder)Activator.CreateInstance(modelBinderType, _settings);
        }
    }
}
