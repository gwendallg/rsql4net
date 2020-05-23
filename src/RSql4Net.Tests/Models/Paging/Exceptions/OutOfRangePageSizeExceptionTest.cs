using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using RSql4Net.Configurations;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Paging.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Paging.Exceptions
{
    public class OutOfRangePageSizeExceptionTest : PageableExceptionTest<OutOfRangePageSizeException>
    {
        [Fact]
        public void ShouldBeOutOfRangePageSizeException()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("PageSize", new StringValues("-1"))
                    }
                ));
            var expected = new PageableModelBinder<object>(new Settings());

            expected
                .Invoking(f => f.Build(queryCollection))
                .Should().Throw<OutOfRangePageSizeException>();
        }
        
        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable();
        }
    }
}
