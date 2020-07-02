using System;
using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonEnumExpressionTest : ComparisonExpressionTest<AttributeTargets>
    {
        protected override AttributeTargets Manifest1()
        {
            return AttributeTargets.Constructor;
        }

        protected override string Manifest1ToString()
        {
            return Manifest1().ToString();
        }

        protected override AttributeTargets Manifest2()
        {
            return AttributeTargets.Assembly;
        }

        protected override string Manifest2ToString()
        {
            return Manifest2().ToString();
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
