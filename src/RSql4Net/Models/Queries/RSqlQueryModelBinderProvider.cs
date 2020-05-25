using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RSql4Net.Configurations;

namespace RSql4Net.Models.Queries
{
    public class RSqlQueryModelBinderProvider : IModelBinderProvider
    {
        private readonly Settings _settings;

        public RSqlQueryModelBinderProvider(Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

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
            return (IModelBinder)Activator.CreateInstance(modelBinderType, _settings);
        }
    }
}
