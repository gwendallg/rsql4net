using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public class QueryComparisonInvalidMatchTypeExceptionTest : QueryExceptionTest
    {
        [Fact]
        public void ShoudBeThrow()
        {
            var query = "Int32P==StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();

            query = "Int32P<StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();

            query = "Int32P=lt=StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();

            query = "Int32P<=StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();

            query = "Int32P=le=StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();

            query = "Int32P>StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();

            query = "Int32P=gt=StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();

            query = "Int32P>=StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();

            query = "Int32P=ge=StringP";
            this
                .Invoking(f => BuildExpression(query))
                .Should()
                .Throw<QueryComparisonInvalidMatchTypeException>();
        }

        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable<QueryComparisonInvalidMatchTypeException>();
        }
    }
}
