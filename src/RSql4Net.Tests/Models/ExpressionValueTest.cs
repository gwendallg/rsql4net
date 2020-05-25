using System;
using System.Linq.Expressions;
using Xunit;
using FluentAssertions;
using RSql4Net.Models;

namespace RSql4Net.Tests.Models
{
    public class ExpressionValueTest
    {
        [Fact]
        public void ShouldBeThrowArgumentNullException()
        {
            // parameter = null
            this.Invoking((a) => ExpressionValue.Parse<Customer>(null, "test", null))
                .Should().Throw<ArgumentNullException>();
            
            // selector = null
            this.Invoking((a) => ExpressionValue.Parse<Customer>(Expression.Parameter(typeof(Customer)), null, null))
                .Should().Throw<ArgumentNullException>();
            
            // selector = string .empty
            this.Invoking((a) => ExpressionValue.Parse<Customer>(Expression.Parameter(typeof(Customer)), String.Empty, null))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
