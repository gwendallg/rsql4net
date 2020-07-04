﻿using System.Globalization;
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
            var query = $"{Helper.GetJsonPropertyName(obj1)}P==\"{obj1ToString}\"";
            // {type}P == value
            var expected = Helper.Function<MockQuery>(query);
            var actual = Actual(obj1);
            Assert.True(expected(actual));
        }

        [Fact]
        public void ShouldBeEqualsSingleQuote()
        {
            var obj1 = Manifest1();
            var obj1ToString = Manifest1ToString();
            var query = $"{Helper.GetJsonPropertyName(obj1)}P=='{obj1ToString}'";
            // {type}P == value
            var expected = Helper.Function<MockQuery>(query);
            var actual = Actual(obj1);
            Assert.True(expected(actual));
        }
        
        [Fact]
        public void ShouldBeEqualsEmptySingleQuote()
        {
            var obj1 = Manifest1();
            var query = $"{Helper.GetJsonPropertyName(obj1)}P==''";
            // {type}P == value
            var expected = Helper.Function<MockQuery>(query);
            var actual = Actual(string.Empty);
            Assert.True(expected(actual));
        } 
        
        [Fact]
        public void ShouldBeEqualsEmptyDoubleQuote()
        {
            var obj1 = Manifest1();
            var query = $"{Helper.GetJsonPropertyName(obj1)}P==\"\"'";
            // {type}P == value
            var expected = Helper.Function<MockQuery>(query);
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
            var query = $"{Helper.GetJsonPropertyName(obj1)}P=in={listManifest}";
            // {type}P == value
            var expected = Helper.Function<MockQuery>(query);
            var actual = Actual(obj1);
            Assert.True(expected(actual));

            obj1 = Manifest1();
            listManifest = ListManifestToStringWithDoubleQuote();
            query = $"{Helper.GetJsonPropertyName(obj1)}P=in={listManifest}";
            // {type}P == value
            expected = Helper.Function<MockQuery>(query);
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
            var query = $"{Helper.GetJsonPropertyName(obj2)}P=out={listManifest}";
            // {type}P == value
            var expected = Helper.Function<MockQuery>(query);
            var actual = Actual("AC");
            Assert.True(expected(actual));


            listManifest = ListManifestToStringWithQuote();
            query = $"{Helper.GetJsonPropertyName(obj2)}P=out={listManifest}";
            // {type}P == value
            expected = Helper.Function<MockQuery>(query);
            actual = Actual("AC");
            Assert.True(expected(actual));


            listManifest = ListManifestToStringWithDoubleQuote();
            query = $"{Helper.GetJsonPropertyName(obj2)}P=out={listManifest}";
            // {type}P == value
            expected = Helper.Function<MockQuery>(query);
            actual = Actual("AC");
            Assert.True(expected(actual));
        }

        [Fact]
        public virtual void ShouldThrowExceptionGreaterThanOrEquals()
        {
            Assert.Throws<ComparisonInvalidComparatorSelectionException>(() => OnShouldBeGreaterThanOrEquals());
        }

        [Fact]
        public virtual void ShouldThrowExceptionGreaterThan()
        {
            Assert.Throws<ComparisonInvalidComparatorSelectionException>(() => OnShouldBeGreaterThan());
        }

        [Fact]
        public virtual void ShouldThrowExceptionLowerThan()
        {
            Assert.Throws<ComparisonInvalidComparatorSelectionException>(() => OnShouldBeLowerThan());
        }

        [Fact]
        public virtual void ShouldThrowExceptionLowerThanOrEquals()
        {
            Assert.Throws<ComparisonInvalidComparatorSelectionException>(() => OnShouldBeLowerThanOrEquals());
        }
    }
}