using FluentAssertions;
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
            var queryCollection = Helper.QueryCollection("sort", "a;t");
            var expected = new RSqlPageableModelBinder<Customer>(Helper.Settings(),Helper.JsonOptions());

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
