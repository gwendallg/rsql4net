using FluentAssertions;
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
            var queryCollection = Helper.QueryCollection("sort", "name;asc;sd");
            var expected = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions());
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
