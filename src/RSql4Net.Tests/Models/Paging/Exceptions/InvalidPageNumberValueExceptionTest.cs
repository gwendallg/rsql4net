using FluentAssertions;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Paging.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Paging.Exceptions
{
    public class InvalidPageNumberValueExceptionTest : PageableExceptionTest<InvalidPageNumberValueException>
    {
        [Fact]
        public void ShouldBeThrow()
        {
            var queryCollection = Helper.QueryCollection("pageNumber", "a");
            var expected = new RSqlPageableModelBinder<object>(Helper.Settings(), Helper.JsonOptions());

            expected
                .Invoking(f => f.Build(queryCollection))
                .Should().Throw<InvalidPageNumberValueException>();
        }

        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable();
        }
    }
}
