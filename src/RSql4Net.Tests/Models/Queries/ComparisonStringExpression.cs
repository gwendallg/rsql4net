using System.Globalization;
using System.Text;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class ComparisonStringExpressionTest : ComparisonExpressionTest<string>
    {
        protected override string Manifest1()
        {
            return "AA";
        }

        protected override string Manifest1ToString()
        {
            return Manifest1().ToString(CultureInfo.InvariantCulture);
        }

        protected override string Manifest2()
        {
            return "AB";
        }

        protected override string Manifest2ToString()
        {
            return Manifest2().ToString(CultureInfo.InvariantCulture);
        }

        protected virtual string ListManifestToStringWithQuote()
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append("'" + Manifest1ToString() + "','");
            builder.Append(Manifest2ToString());
            builder.Append("')");
            return builder.ToString();
        }

        protected virtual string ListManifestToStringWithDoubleQuote()
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append("\"" + Manifest1ToString() + "\",\"");
            builder.Append(Manifest2ToString());
            builder.Append("\")");
            return builder.ToString();
        }

        [Fact]
        public void ShouldBeEqualsDoubleQuote()
        {
            var obj1 = Manifest1();
            var obj1ToString = Manifest1ToString();
            var query = $"{obj1.GetType().Name}P==\"{obj1ToString}\"";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1);
            Assert.True(expected(actual));
        }

        [Fact]
        public void ShouldBeEqualsSingleQuote()
        {
            var obj1 = Manifest1();
            var obj1ToString = Manifest1ToString();
            var query = $"{obj1.GetType().Name}P=='{obj1ToString}'";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1);
            Assert.True(expected(actual));
        }
        
        [Fact]
        public void ShouldBeEqualsEmptySingleQuote()
        {
            var obj1 = Manifest1();
            var query = $"{obj1.GetType().Name}P==''";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(string.Empty);
            Assert.True(expected(actual));
        } 
        
        [Fact]
        public void ShouldBeEqualsEmptyDoubleQuote()
        {
            var obj1 = Manifest1();
            var query = $"{obj1.GetType().Name}P==\"\"'";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(string.Empty);
            Assert.True(expected(actual));
        }
        [Fact]
        public virtual void ShouldBeEquals()
        {
            OnShouldBeEquals();
        }

        [Fact]
        public virtual void ShouldBeIn()
        {
            OnShouldBeIn();

            var obj1 = Manifest1();
            var listManifest = ListManifestToStringWithQuote();
            var query = $"{obj1.GetType().Name}P=in={listManifest}";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1);
            Assert.True(expected(actual));

            obj1 = Manifest1();
            listManifest = ListManifestToStringWithDoubleQuote();
            query = $"{obj1.GetType().Name}P=in={listManifest}";
            // {type}P == value
            expected = BuildExpression(query);
            actual = Actual(obj1);
            Assert.True(expected(actual));
        }

        [Fact]
        public virtual void ShouldBeNotEquals()
        {
            OnShouldBeNotEquals();
        }

        [Fact]
        public virtual void ShouldBeNotIn()
        {
            var obj2 = Manifest2();
            var listManifest = ListManifestToString();
            var query = $"{obj2.GetType().Name}P=out={listManifest}";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual("AC");
            Assert.True(expected(actual));


            listManifest = ListManifestToStringWithQuote();
            query = $"{obj2.GetType().Name}P=out={listManifest}";
            // {type}P == value
            expected = BuildExpression(query);
            actual = Actual("AC");
            Assert.True(expected(actual));


            listManifest = ListManifestToStringWithDoubleQuote();
            query = $"{obj2.GetType().Name}P=out={listManifest}";
            // {type}P == value
            expected = BuildExpression(query);
            actual = Actual("AC");
            Assert.True(expected(actual));
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
