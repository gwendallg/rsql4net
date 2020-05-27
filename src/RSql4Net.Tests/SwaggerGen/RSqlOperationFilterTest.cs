using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Moq;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;
using RSql4Net.SwaggerGen;
using RSql4Net.Tests.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace RSql4Net.Tests.SwaggerGen
{
    public class RSqlOperationFilterTest
    {
        [Fact]
        public void ShouldBeThrowArgumentNullException()
        {
            this.Invoking(a => new RSqlOperationFilter(null, Helper.JsonOptions()))
                .Should().Throw<ArgumentNullException>();
        }

        private void MockQueryMethod(IRSqlQuery<Customer> query) { }
        private void MockPageableMethod(IRSqlPageable<Customer> pageable) { }

        [Fact]
        public void ShouldBeWithQueryParameter()
        {
            var apiDescription = new ApiDescription();
            var schemaGenerator = new Mock<ISchemaGenerator>();
            var schemaRepository = new SchemaRepository();
            var operationFilterContext = new OperationFilterContext(
                apiDescription,
                schemaGenerator.Object,
                schemaRepository,
                GetType().GetMethod("MockQueryMethod", BindingFlags.NonPublic | BindingFlags.Instance));

            var openApiOperation = new OpenApiOperation();
            openApiOperation.Parameters.Add(new OpenApiParameter() {Name = "query"});

            var settings = Helper.Settings();
            var option = Helper.JsonOptions();
            
            var operationFilter = new RSqlOperationFilter(settings, Helper.JsonOptions());
            operationFilter.Apply(openApiOperation, operationFilterContext);

            // one parameter
            openApiOperation.Parameters.Count()
                .Should().Be(1);
            
            var expected = openApiOperation.Parameters.First();
            
            // In = ParameterLocation.Query
            expected.In
                .Should().Be(ParameterLocation.Query);
            
            // Name = settings.QueryField
            expected.Name
                .Should().Be(option.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(settings.QueryField));
            // schema = string
            expected.Schema.Type
                .Should().Be("string");
        }

        [Fact]
        public void ShouldBeWithPageableParameter()
        {
            var apiDescription = new ApiDescription();
            var schemaGenerator = new Mock<ISchemaGenerator>();
            var schemaRepository = new SchemaRepository();
            var operationFilterContext = new OperationFilterContext(
                apiDescription,
                schemaGenerator.Object,
                schemaRepository,
                GetType().GetMethod("MockPageableMethod", BindingFlags.NonPublic | BindingFlags.Instance));

            var openApiOperation = new OpenApiOperation();
            openApiOperation.Parameters.Add(new OpenApiParameter() {Name = "pageable"});

            var settings = Helper.Settings();
            var option = Helper.JsonOptions();
            
            var operationFilter = new RSqlOperationFilter(settings,option);
            operationFilter.Apply(openApiOperation, operationFilterContext);

            // one parameter
            openApiOperation.Parameters.Count()
                .Should().Be(3);

            // pageSize
            var expected = openApiOperation.Parameters.SingleOrDefault(p => p.Name == option.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(settings.PageSizeField));
            expected.In
                .Should().Be(ParameterLocation.Query);
            // schema = number
            expected.Schema.Type
                .Should().Be("number");
            //  default = setting.PageSize
            expected.Schema.Default.As<OpenApiInteger>().Value
                .Should().Be(settings.PageSize);
            
            // pageNumber
            expected = openApiOperation.Parameters.SingleOrDefault(p => p.Name == option.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(settings.PageNumberField));
            expected.In
                .Should().Be(ParameterLocation.Query);
            // schema = number
            expected.Schema.Type
                .Should().Be("number");
            //  default = 0
            expected.Schema.Default.As<OpenApiInteger>().Value
                .Should().Be(0);
            
            // sort
            expected = openApiOperation.Parameters.SingleOrDefault(p => p.Name == option.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(settings.SortField));
            expected.In
                .Should().Be(ParameterLocation.Query);
            // schema = number
            expected.Schema.Type
                .Should().Be("array");
            //  default = 0
            expected.Schema.Items.Type
                .Should().Be("string");
        }

    }
}
