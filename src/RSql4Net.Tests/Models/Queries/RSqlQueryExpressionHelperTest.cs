using System;
using System.Text.Json;
using FluentAssertions;
using RSql4Net.Models.Queries;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class RSqlQueryExpressionHelperTest
    {
        [Fact]
        public void ShouldBeTrue()
        {
            var expected = RSqlQueryExpressionHelper.True<Customer>();
            expected.Compile()(new Customer())
                .Should().BeTrue();
        }

        [Fact]
        public void ShouldBeGetAndExpressionThrowArgumentNullExceptionTest()
        {
            // visitor = null
            var context = new RSqlQueryParser.AndContext(null,0);
            this.Invoking(a => RSqlQueryExpressionHelper.GetAndExpression<Customer>(null, context))
                .Should().Throw<ArgumentNullException>();
            
            var visitor = new RSqlDefaultQueryVisitor<Customer>(JsonNamingPolicy.CamelCase);
            // context = null
            this.Invoking(a => RSqlQueryExpressionHelper.GetAndExpression(visitor, null))
                .Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void ShouldBeGetOrExpressionThrowArgumentNullExceptionTest()
        {
            // visitor = null
            var context = new RSqlQueryParser.OrContext(null,0);
            this.Invoking(a => RSqlQueryExpressionHelper.GetOrExpression<Customer>(null, context))
                .Should().Throw<ArgumentNullException>();
            
            var visitor = new RSqlDefaultQueryVisitor<Customer>(JsonNamingPolicy.CamelCase);
            // context = null
            this.Invoking(a => RSqlQueryExpressionHelper.GetOrExpression(visitor, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldBeGetIsNullExpressionThrowComparisonInvalidComparatorSelectionExceptionTest()
        {
            const int obj1 = 1;
            this.Invoking(a => Helper.Expression<MockQuery>($"{Helper.GetJsonPropertyName(obj1)}P=is-null=true"))
                .Should()
                .Throw<ComparisonInvalidComparatorSelectionException>();
        }
        
        [Fact]
        public void ShouldBeGetIsNullExpressionThrowComparisonNotEnoughArgumentExceptionTest()
        {
            const int obj1 = 1;
            this.Invoking(a => Helper.Expression<MockQuery>($"{Helper.GetJsonPropertyName(obj1)}NullP=is-null="))
                .Should()
                .Throw<ComparisonNotEnoughArgumentException>();
        }
        
        [Fact]
        public void ShouldBeGetIsNullExpressionThrowComparisonTooManyArgumentExceptionTest()
        {
            const string obj1 = "test";
            this.Invoking(a => Helper.Expression<MockQuery>($"{Helper.GetJsonPropertyName(obj1)}P=is-null=(true,false)"))
                .Should()
                .Throw<ComparisonTooManyArgumentException>();
        }

        [Fact]
        public void ShouldBeGetLkExpressionThrowComparisonTooManyArgumentExceptionTest()
        {
            const string obj1 = "test";
            this.Invoking(a => Helper.Expression<MockQuery>($"{Helper.GetJsonPropertyName(obj1)}P==(true,false)"))
                .Should()
                .Throw<ComparisonTooManyArgumentException>();
        }

        [Fact]
        public void ShouldBeGetEqExpressionThrowComparisonInvalidComparatorSelectionExceptionTest()
        {
            this.Invoking(a => Helper.Expression<MockQuery>($"childP==true"))
                .Should()
                .Throw<ComparisonInvalidComparatorSelectionException>();
        }
    }
}
