using FluentAssertions;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Paging.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Paging.Exceptions
{
    public class InvalidPageSizeValueExceptionTest: PageableExceptionTest<InvalidPageSizeValueException>
    {
        [Fact]
        public void ShouldBeInvalidPageSizeValueException()
        {
            var queryCollection = Helper.QueryCollection("pageSize","a");
            var expected = new RSqlPageableModelBinder<object>(Helper.Settings(),Helper.JsonOptions());

            expected
                .Invoking(f => f.Build(queryCollection))
                .Should().Throw<InvalidPageSizeValueException>();
        }
        
        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable();
        }
    }
}
