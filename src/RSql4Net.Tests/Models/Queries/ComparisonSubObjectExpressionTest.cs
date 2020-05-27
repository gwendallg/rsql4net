using FluentAssertions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonSubObjectExpressionTest
    {
        [Fact]
        public void ShouldBeEquals()
        {
            const string actual = "Param_0 => (Param_0.ChildP.StringP == \"a\")";
            var query = "childP.stringP==a";
            var expected = Helper.Expression<MockQuery>(query).ToString();
            expected
                .Should().Be(actual);
          
            // =eq=
            query = "childP.stringP=eq=a";
            expected = Helper.Expression<MockQuery>(query).ToString();
            expected
                .Should().Be(actual);
        }
    }
}
