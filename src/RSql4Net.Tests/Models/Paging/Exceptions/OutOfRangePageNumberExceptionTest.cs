using FluentAssertions;
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
            var queryCollection = Helper.QueryCollection("pageNumber", "-1");
            var expected = new RSqlPageableModelBinder<object>(Helper.Settings(), Helper.JsonOptions(), Helper.MockLogger<object>().Object);

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
