using System;
using FluentAssertions;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class QueryTest
    {
        [Fact]
        public void ShouldBeThrowArgumentNullExceptionTest()
        {
            // ArgumentNullException : obj
            this.Invoking(f => { var query = new RSqlQuery<Customer>(null); })
                .Should().Throw<ArgumentNullException>();
        }
    }
}
