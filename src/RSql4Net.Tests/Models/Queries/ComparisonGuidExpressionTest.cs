using System;
using System.Text;
using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonGuidExpressionTest : ComparisonExpressionTest<Guid>
    {
        private readonly Guid _manifest1 = Guid.NewGuid();
        private readonly Guid _manifest2 = Guid.NewGuid(); 
        protected override Guid Manifest1()
        {
            return _manifest1;
        }

        protected override string Manifest1ToString()
        {
            return Manifest1().ToString();
        }

        protected override Guid Manifest2()
        {
            return _manifest2;
        }

        protected override string Manifest2ToString()
        {
            return Manifest2().ToString();
        }

        protected override string ListManifestToString()
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(Manifest1ToString());
            builder.Append(")");
            return builder.ToString();
        }

        [Fact]
        public void ShouldBeEquals()
        {
            OnShouldBeEquals();
        }

        [Fact]
        public void ShouldBeEqualsWithNullable()
        {
            OnShouldBeEqualsWithNullable();
        }

        [Fact]
        public void ShouldBeIn()
        {
            OnShouldBeIn();
        }

        [Fact]
        public void ShouldBeInNullable()
        {
            OnShouldBeInNullable();
        }

        [Fact]
        public void ShouldBeNotEquals()
        {
            OnShouldBeNotEquals();
        }

        [Fact]
        public void ShouldBeNotEqualsWithNullable()
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
        public virtual void ShouldThrowExceptionGreaterThanOrEquals()
        {
            this.Invoking(o => o.OnShouldBeGreaterThanOrEquals())
                .Should().Throw<ComparisonInvalidComparatorSelectionException>();
        }

        [Fact]
        public virtual void ShouldThrowExceptionGreaterThan()
        {
            Assert.Throws<ComparisonInvalidComparatorSelectionException>(OnShouldBeGreaterThan);
        }

        [Fact]
        public virtual void ShouldThrowExceptionLowerThan()
        {
            Assert.Throws<ComparisonInvalidComparatorSelectionException>(OnShouldBeLowerThan);
        }

        [Fact]
        public virtual void ShouldThrowExceptionLowerThanOrEquals()
        {
            Assert.Throws<ComparisonInvalidComparatorSelectionException>(OnShouldBeLowerThanOrEquals);
        }
    }
} 
