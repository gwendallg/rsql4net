using System;
using System.Globalization;
using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonDoubleExpressionTest : ComparisonExpressionTest<double>
    {
        protected override double Manifest1()
        {
            return Math.Round(1.1,1);
        }

        protected override string Manifest1ToString()
        {
            return Manifest1().ToString(NumberFormatInfo.InvariantInfo);
        }

        protected override double Manifest2()
        {
            return Math.Round(1.2,1);
        }

        protected override string Manifest2ToString()
        {
            return Manifest2().ToString(NumberFormatInfo.InvariantInfo);
        }

        [Fact]
        public virtual void ShouldBeEquals()
        {
            OnShouldBeEquals();
        }

        [Fact]
        public virtual void ShouldBeEqualsWithNullable()
        {
            OnShouldBeEqualsWithNullable();
        }

        [Fact]
        public virtual void ShouldBeGreaterThan()
        {
            OnShouldBeGreaterThan();
        }

        [Fact]
        public virtual void ShouldBeGreaterThanOrEquals()
        {
            OnShouldBeGreaterThanOrEquals();
        }

        [Fact]
        public virtual void ShouldBeGreaterThanOrEqualsWithNullable()
        {
            OnShouldBeGreaterThanOrEqualsWithNullable();
        }

        [Fact]
        public virtual void ShouldBeGreaterThanWithNullable()
        {
            OnShouldBeGreaterThanWithNullable();
        }

        [Fact]
        public virtual void ShouldBeLowerThan()
        {
            OnShouldBeLowerThan();
        }

        [Fact]
        public virtual void ShouldBeLowerThanOrEquals()
        {
            OnShouldBeLowerThanOrEquals();
        }

        [Fact]
        public virtual void ShouldBeLowerThanOrEqualsWithNullable()
        {
            OnShouldBeLowerThanOrEqualsWithNullable();
        }

        [Fact]
        public virtual void ShouldBeLowerThanWithNullable()
        {
            OnShouldBeLowerThanWithNullable();
        }

        [Fact]
        public virtual void ShouldBeNotEquals()
        {
            OnShouldBeNotEquals();
        }

        [Fact]
        public virtual void ShouldBeNotEqualsWithNullable()
        {
            OnShouldBeNotEqualsWithNullable();
        }

        [Fact]
        public virtual void ShouldBeNotIn()
        {
            OnShouldBeNotIn();
        }

        [Fact]
        public virtual void ShouldBeNotInNullable()
        {
            OnShouldBeNotInNullable();
        }

        [Fact]
        public virtual void ShouldBeIn()
        {
            OnShouldBeIn();
        }
     
        [Fact]
        public virtual void ShouldBeInNullable()
        {
            OnShouldBeInNullable();
        }
        
        [Fact]
        public void ShouldThrowInvalidConversionException()
        {
            this.Invoking(_ => OnShouldThrowInvalidConversionException("a"))
                .Should()
                .Throw<InvalidConversionException>();
        }
    }
}
