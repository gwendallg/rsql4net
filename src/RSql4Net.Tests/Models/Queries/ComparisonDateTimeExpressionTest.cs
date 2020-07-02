using System;
using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonDateTimeExpressionTest : ComparisonExpressionTest<DateTime>
    {
        protected override DateTime Manifest1()
        {
            return DateTime.Today;
        }

        protected override string Manifest1ToString()
        {
            return Manifest1().ToString("s");
        }

        protected override DateTime Manifest2()
        {
            return DateTime.Today.AddDays(1);
        }

        protected override string Manifest2ToString()
        {
            return Manifest2().ToString("s");
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
        public void ShouldThrowInvalidConversionException()
        {
            this.Invoking(s => OnShouldThrowInvalidConversionException("a"))
                .Should()
                .Throw<InvalidConversionException>();
        }
    }
}
