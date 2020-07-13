using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class QueryModelBinderProviderTest
    {

        [Fact]
        public void ShouldBeNotQueryModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(string));
            var expected = new RSqlQueryModelBinderProvider();
            expected.GetBinder(modelBinderProviderContextMock)
                .Should().BeNull();
            
            modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(int?));
            expected = new RSqlQueryModelBinderProvider();
            expected.GetBinder(modelBinderProviderContextMock)
                .Should().BeNull();
        }

        [Fact]
        public void ShouldBeQueryModelBinder()
        {
            var modelBinderProviderContextMock =
                new MockModelBinderProviderContext(typeof(RSqlQuery<Customer>));
            var expected = new RSqlQueryModelBinderProvider();
            expected.GetBinder(modelBinderProviderContextMock)
                .Should().BeOfType<BinderTypeModelBinder>();
        }
    }
}
