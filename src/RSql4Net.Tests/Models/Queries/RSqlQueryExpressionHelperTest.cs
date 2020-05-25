using System;
using System.Linq.Expressions;
using FluentAssertions;
using Newtonsoft.Json.Serialization;
using RSql4Net.Models;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class RSqlQueryExpressionHelperTest
    {
        [Fact]
        public void ShouldBeGetAndExpressionThrowArgumentNullExceptionTest()
        {
            // visitor = null
            var context = new RSqlQueryParser.AndContext(null,0);
            this.Invoking(a => RSqlQueryExpressionHelper.GetAndExpression<Customer>(null, context))
                .Should().Throw<ArgumentNullException>();
            
            var visitor = new RSqlDefaultQueryVisitor<Customer>(new CamelCaseNamingStrategy());
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
            
            var visitor = new RSqlDefaultQueryVisitor<Customer>(new CamelCaseNamingStrategy());
            // context = null
            this.Invoking(a => RSqlQueryExpressionHelper.GetOrExpression<Customer>(visitor, null))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
