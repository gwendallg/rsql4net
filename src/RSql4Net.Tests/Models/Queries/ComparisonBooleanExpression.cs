using System.Text;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonBooleanExpressionTest : ComparisonExpressionTest<bool>
    {
        protected override bool Manifest1()
        {
            return true;
        }

        protected override string Manifest1ToString()
        {
            return Manifest1().ToString();
        }

        protected override bool Manifest2()
        {
            return false;
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
        public virtual void ShouldThrowExceptioneGreaterThanOrEquals()
        {
            Assert.Throws<QueryComparisonInvalidComparatorSelectionException>(() => OnShouldBeGreaterThanOrEquals());
        }

        [Fact]
        public virtual void ShouldThrowExceptionGreaterThan()
        {
            Assert.Throws<QueryComparisonInvalidComparatorSelectionException>(() => OnShouldBeGreaterThan());
        }

        [Fact]
        public virtual void ShouldThrowExceptionLowerThan()
        {
            Assert.Throws<QueryComparisonInvalidComparatorSelectionException>(() => OnShouldBeLowerThan());
        }

        [Fact]
        public virtual void ShouldThrowExceptionLowerThanOrEquals()
        {
            Assert.Throws<QueryComparisonInvalidComparatorSelectionException>(() => OnShouldBeLowerThanOrEquals());
        }
    }
}
