using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RSql4Net.Configurations;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RSql4Net.SwaggerGen
{
    /// <summary>
    /// 
    /// </summary>
    public class RSqlOperationFilter : IOperationFilter
    {
        private readonly Settings _settings;
        private readonly IOptions<JsonOptions> _options;

        /// <summary>
        /// is query type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsQuery(Type type) => (type.IsGenericType &&
                                                   (type.GetGenericTypeDefinition() == typeof(IRSqlQuery<>) ||
                                                    type.GetGenericTypeDefinition() == typeof(RSqlQuery<>)));
        /// <summary>
        /// is pageable type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsPageable(Type type) => (type.IsGenericType &&
                                                      (type.GetGenericTypeDefinition() == typeof(IRSqlPageable<>) ||
                                                       type.GetGenericTypeDefinition() == typeof(RSqlPageable<>)));
        
        /// <summary>
        /// create instance of
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="options"></param>
        public RSqlOperationFilter(Settings settings, IOptions<JsonOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
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
                Name = _options.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(_settings.QueryField),
                Schema = new OpenApiSchema() {Type = "string"}
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
                Name = _options.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(_settings.PageNumberField),
                Schema = new OpenApiSchema()
                {
                    Type = "number", Default = new OpenApiInteger(0), Description = "Page number"
                }
            };
            operation.Parameters.Add(parameter);

            parameter = new OpenApiParameter
            {
                In = ParameterLocation.Query,
                Name = _options.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(_settings.PageSizeField),
                Schema = new OpenApiSchema()
                {
                    Type = "number", Default = new OpenApiInteger(_settings.PageSize), Description = "Page size"
                }
            };
            operation.Parameters.Add(parameter);

            parameter = new OpenApiParameter
            {
                In = ParameterLocation.Query,
                Name = _options.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(_settings.SortField),
                Schema = new OpenApiSchema()
                {
                    Type = "array", Items = new OpenApiSchema() {Type = "string", Description = "Sort"}
                }
            };
            operation.Parameters.Add(parameter);
        }
    }
}
