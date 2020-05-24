using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RSql4Net.Configurations;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Paging.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Paging.Exceptions
{
    public class InvalidSortDirectionExceptionTest : PageableExceptionTest<InvalidSortDirectionException>
    {
        [Fact]
        public void ShouldBeInvalidSortDirectionException()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("Sort", new StringValues("Name;asc;sd"))
                    }
                ));
            var expected = new PageableModelBinder<Customer>(new Settings());

            expected
                .Invoking(i => i.Build(queryCollection))
                .Should().Throw<InvalidSortDirectionException>();
        }
        
        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable();
        }
    }
}
