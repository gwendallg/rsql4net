using Xunit;
using FluentAssertions;
using RSql4Net.Models;

namespace RSql4Net.Tests.Models
{
    public class MethodContainsInfoTest
    {
        [Fact]
        public void ShouldBeReturnNull()
        {
            var expected = new MethodContainsInfo(typeof(Customer));
            expected.Convert(null).Should().BeNull();
        }
    }
}
