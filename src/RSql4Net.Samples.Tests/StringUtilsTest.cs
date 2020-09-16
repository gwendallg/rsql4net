using FluentAssertions;
using Xunit;

namespace RSql4Net.Samples.Tests
{
    public class StringUtilsTest
    {
        [Fact]
        public void ShouldBeSnakeCase()
        {
            var actual = "should_be_snake_case";
            var expected = StringUtils.ToSnakeCase("ShouldBeSnakeCase");
            expected.Should().Be(actual);
        }
    }
}
