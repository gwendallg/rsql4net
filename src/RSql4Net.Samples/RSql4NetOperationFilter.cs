using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using RSql4Net.Configurations;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.OpenApi.Any;

namespace RSql4Net.Samples
{
    public class RSql4NetOperationFilter : IOperationFilter
    {
        private readonly Settings _settings;

        private bool IsQuery(Type type) => (type.IsGenericType &&
                                            (type.GetGenericTypeDefinition() == typeof(IQuery<>) ||
                                             type.GetGenericTypeDefinition() == typeof(Query<>)));

        private bool IsPageable(Type type) => (type.IsGenericType &&
                                               (type.GetGenericTypeDefinition() == typeof(IPageable<>) ||
                                                type.GetGenericTypeDefinition() == typeof(Pageable<>)));

        public RSql4NetOperationFilter(Settings settings)
        {
            _settings = settings;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var parameterInfo in context.MethodInfo.GetParameters())
            {
                if (IsQuery(parameterInfo.ParameterType))
                {
                    BuildQueryParameters(parameterInfo, operation);
                }
                else if (IsPageable(parameterInfo.ParameterType))
                {
                    BuildPageableParameters(parameterInfo, operation);
                }
            }
        }

        private void BuildQueryParameters(ParameterInfo parameterInfo, OpenApiOperation operation)
        {
            var parameter = operation.Parameters.Single(p => p.Name == parameterInfo.Name);
            operation.Parameters.Remove(parameter);

            parameter = new OpenApiParameter
            {
                In = ParameterLocation.Query,
                Name = _settings.QueryField,
                Schema = new OpenApiSchema()
                {
                    Type = "string"
                }
                
            };
            operation.Parameters.Add(parameter);

        }

        private void BuildPageableParameters(ParameterInfo parameterInfo, OpenApiOperation operation)
        {
            var parameter = operation.Parameters.Single(p => p.Name == parameterInfo.Name);
            operation.Parameters.Remove(parameter);
            parameter = new OpenApiParameter
            {
                In = ParameterLocation.Query,
                Name = _settings.PageNumberField,
                Schema = new OpenApiSchema()
                {
                    Type = "number",
                    Default = new OpenApiInteger(0),
                    Description = "Page number"
                }
                
            };
            operation.Parameters.Add(parameter);
            
            parameter = new OpenApiParameter
            {
                In = ParameterLocation.Query,
                Name = _settings.PageSizeField,
                Schema = new OpenApiSchema()
                {
                    Type = "number",
                    Default = new OpenApiInteger(_settings.PageSize),
                    Description = "Page size"
                }
                
            };
            operation.Parameters.Add(parameter);
            
            parameter = new OpenApiParameter
            {
                In = ParameterLocation.Query,
                Name = _settings.SortField,
                Schema = new OpenApiSchema()
                {
                    Type = "array",
                    Items = new OpenApiSchema()
                    {
                        Type = "string",
                        Description = "Sort"
                    }
                    
                }
                
            };
            operation.Parameters.Add(parameter);
        }
    }
}
