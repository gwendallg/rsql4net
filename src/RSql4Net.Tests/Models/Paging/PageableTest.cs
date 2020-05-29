using FluentAssertions;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Paging.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Paging
{
    public class PageableTest
    {
        [Fact]
        public void ShouldBeThrowInvalidPageNumberValueException()
        {
            this.Invoking(o => new RSqlPageable<Customer>(-1, 1))
                .Should()
                .Throw<InvalidPageNumberValueException>();
        }

        [Fact]
        public void ShouldBeThrowInvalidPageSizeValueException()
        {
            this.Invoking(o => new RSqlPageable<Customer>(0, 0))
                .Should()
                .Throw<InvalidPageSizeValueException>();           
        }
        
    }
    
}
