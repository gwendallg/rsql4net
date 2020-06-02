using System;
using System.Linq.Expressions;
using System.Text.Json;
using FluentAssertions;
using Moq;
using RSql4Net.Models.Queries;
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
            this.Invoking(a => RSqlQueryExpressionHelper.GetAndExpression<Customer>(visitor, null))
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
            this.Invoking(a => RSqlQueryExpressionHelper.GetOrExpression<Customer>(visitor, null))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
