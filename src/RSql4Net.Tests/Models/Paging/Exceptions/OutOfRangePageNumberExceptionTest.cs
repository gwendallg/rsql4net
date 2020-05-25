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
    public class OutOfRangePageNumberExceptionTest : PageableExceptionTest<OutOfRangePageNumberException>
    {
        [Fact]
        public void ShouldBeOutOfRangePageNumberException()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("PageNumber", new StringValues("-1"))
                    }
                ));
            var expected = new RSqlPageableModelBinder<object>(new Settings());

            expected
                .Invoking(f => f.Build(queryCollection))
                .Should().Throw<OutOfRangePageNumberException>();
        }
        
        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable();
        }
    }
}
