using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public class ComparisonInvalidMatchTypeExceptionTest : QueryExceptionTest
    {
        [Fact]
        public void ShouldBeThrow()
        {
            var query = "int32P==stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();

            query = "int32P<stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();

            query = "int32P=lt=stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();

            query = "int32P<=stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();

            query = "int32P=le=stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();

            query = "int32P>stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();

            query = "int32P=gt=stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();
            
            query = "int32P>=stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();

            query = "int32P=ge=stringP";
            this
                .Invoking(f => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<ComparisonInvalidMatchTypeException>()
                .And
                .Origin
                .Should()
                .NotBeNull();
        }

        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable<ComparisonInvalidMatchTypeException>();
        }
    }
}
