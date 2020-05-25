using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonExceptionTest : CommonQueryTest
    {
        [Fact]
        public void ShoudBeThrowTooManyArgumentException()
        {
            var query = "Int32P==(1,3)";
            Assert.Throws<QueryComparisonTooManyArgumentException>(() => { BuildExpression(query); });
        }

        [Fact]
        public void ShoudBeThrowUnknownSelectorException()
        {
            var query = "Int32==1";
            Assert.Throws<QueryComparisonUnknownSelectorException>(() => { BuildExpression(query); });
        }

        [Fact]
        public void ShouldThrowUnknownComparatorException()
        {
            var query = "StringP=t=2";
            Assert.Throws<QueryComparisonUnknownComparatorException>(() => { BuildExpression(query); });
        }
    }
}
