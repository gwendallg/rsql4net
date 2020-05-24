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
    public class UnknownSortExceptionTest : PageableExceptionTest<UnknownSortException>
    {
        [Fact]
        public void ShouldBeUnknownSortException()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("Sort", new StringValues("a;t"))
                    }
                ));
            var expected = new PageableModelBinder<Customer>(new Settings());

            expected
                .Invoking(f => f.Build(queryCollection))
                .Should().Throw<UnknownSortException>();
        }
        
        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable();
        }
    }
}
