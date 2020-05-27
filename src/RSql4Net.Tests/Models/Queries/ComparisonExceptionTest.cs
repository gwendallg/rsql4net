using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonExceptionTest 
    {
        [Fact]
        public void ShouldBeThrowQueryComparisonTooManyArgumentException()
        {
            const string query = "int32P==(1,3)";
            this.Invoking(o => Helper.Function<MockQuery>(query))
                .Should().Throw<ComparisonTooManyArgumentException>();
        }

        [Fact]
        public void ShouldBeThrowQueryComparisonUnknownSelectorException()
        {
            const string query = "int32==1";
            this.Invoking(o => Helper.Function<MockQuery>(query))
                .Should().Throw<ComparisonUnknownSelectorException>();
        }

        [Fact]
        public void ShouldThrowQueryComparisonUnknownComparatorException ()
        {
            const string query = "stringP=t=2";
            this.Invoking(o => Helper.Function<MockQuery>(query))
                .Should().Throw<ComparisonUnknownComparatorException >();
        }
    }
}
