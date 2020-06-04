using System;
using FluentAssertions;
using RSql4Net.Samples.Models;
using Xunit;

namespace RSql4Net.Samples.Tests.Models
{
    public class ErrorModelTest
    {
        [Fact]
        public void ShouldBeThrowArgumentNullException()
        {
            this.Invoking(a => new ErrorModel(null))
                .Should().Throw<ArgumentException>();
        }
    }
}
