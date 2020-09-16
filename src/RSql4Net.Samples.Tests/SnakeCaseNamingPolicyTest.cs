using FluentAssertions;
using Xunit;

namespace RSql4Net.Samples.Tests
{
    public class SnakeCaseNamingPolicyTest
    {
        [Fact]
        public void ShouldBeConvertNameValid()
        {
            var actual = "should_be_convert_name_valid";
            var expected = SnakeCaseNamingPolicy.Instance.ConvertName("ShouldBeConvertNameValid");
            expected.Should().Be(actual);
        }
    }
}
