using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonSubObjectExpressionTest : QueryTest
    {
        [Fact]
        public void ShouldBeEquals()
        {
            // ==
            var actual = "Param_0 => (Param_0.ChildP.StringP == \"a\")";
            var query = "ChildP.StringP==a";

            var expected = BuildExpression(query).ToString();
            Assert.True(actual == expected);

            // =eq=
            query = "ChildP.StringP=eq=a";

            expected = BuildExpression(query).ToString();
            Assert.True(actual == expected);
        }
    }
}
