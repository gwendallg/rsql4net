using System;
using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonTimeSpanExpressionTest : ComparisonExpressionTest<TimeSpan>
    {
        protected override TimeSpan Manifest1()
        {
            return DateTime.Today.TimeOfDay;
        }

        protected override TimeSpan Manifest2()
        {
            return Manifest1().Add(TimeSpan.FromHours(1));
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
        public void ShouldThrowInvalidConversionException()
        {
            this.Invoking(s => OnShouldThrowInvalidConversionException("a"))
                .Should()
                .Throw<InvalidConversionException>();
        }
    }
}
