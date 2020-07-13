using FluentAssertions;
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
            var queryCollection = Helper.QueryCollection("pageSize", "-1");
            var expected = new RSqlPageableModelBinder<object>(Helper.Settings(), Helper.JsonOptions(), Helper.MockLogger<object>().Object);

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
