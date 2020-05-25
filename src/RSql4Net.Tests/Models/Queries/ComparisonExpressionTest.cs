using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public abstract class ComparisonExpressionTest<T>
    {
        protected ComparisonExpressionTest()
        {
            Settings = new Settings {QueryField = "q"};
            RSqlQueryModelBinder = new RSqlQueryModelBinder<MockQuery>(Settings);
        }

        protected Settings Settings { get; }
        protected RSqlQueryModelBinder<MockQuery> RSqlQueryModelBinder { get; }

        protected Func<MockQuery, bool> BuildExpression(string query)
        {
            var dic = new Dictionary<string, StringValues> {[Settings.QueryField] = query};
            return RSqlQueryModelBinder
                .Build(new QueryCollection(dic))
                .Value()
                .Compile();
        }

        protected abstract T Manifest1();

        protected virtual string Manifest1ToString()
        {
            return Manifest1().ToString();
        }

        protected abstract T Manifest2();

        protected virtual string Manifest2ToString()
        {
            return Manifest2().ToString();
        }


        protected virtual string ListManifestToString()
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(Manifest1ToString() + ",");
            builder.Append(Manifest2ToString());
            builder.Append(")");
            return builder.ToString();
        }

        protected virtual List<T> ListManifest()
        {
            return new List<T> {Manifest1(), Manifest2()};
        }

        protected MockQuery Actual(object obj, bool nullable = false)
        {
            var property = typeof(MockQuery).GetProperty(obj.GetType().Name + (nullable ? "Null" : "") + "P");
            var result = new MockQuery();
            property.SetValue(result, obj);
            return result;
        }


        protected void OnShouldBeIn(char? closure = null)
        {
            var obj1 = Manifest1();
            var listManifest = ListManifestToString();
            var query = $"{obj1.GetType().Name}P=in={listManifest}";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1);
            Assert.True(expected(actual));
        }

        protected void OnShouldBeInNullable()
        {
            var obj1 = Manifest1();
            var listManifest = ListManifestToString();
            var query = $"{obj1.GetType().Name}NullP=in={listManifest}";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1, true);
            Assert.True(expected(actual));
        }

        protected void OnShouldBeNotIn()
        {
            var obj2 = Manifest2();
            var listManifest = ListManifestToString();
            var query = $"{obj2.GetType().Name}P=out={listManifest}";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(default(T));
            Assert.True(expected(actual));
        }

        protected void OnShouldBeNotInNullable()
        {
            var obj2 = Manifest2();
            var listManifest = ListManifestToString();
            var query = $"{obj2.GetType().Name}NullP=out={listManifest}";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(default(T), true);
            Assert.True(expected(actual));
        }

        #region Equals

        protected virtual void OnShouldBeEquals()
        {
            var obj1 = Manifest1();
            var obj1ToString = Manifest1ToString();
            // {type}P == value
            var query = $"{obj1.GetType().Name}P=={obj1ToString}";
            var expected = BuildExpression(query);
            var actual = Actual(obj1);
            Assert.True(expected(actual));

            // {type}P =eq= value
            query = $"{obj1.GetType().Name}P=eq={obj1ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        protected virtual void OnShouldBeEqualsWithNullable()
        {
            var obj1 = Manifest1();
            var obj1ToString = Manifest1ToString();
            var query = $"{obj1.GetType().Name}NullP=={obj1ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1, true);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj1.GetType().Name}NullP=eq={obj1ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        #endregion

        #region NotEquals

        protected virtual void OnShouldBeNotEquals()
        {
            var obj1 = Manifest1();
            var obj2 = Manifest2();
            var obj1ToString = Manifest1ToString();
            var query = $"{obj1.GetType().Name}P!={obj1ToString}";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(obj2);
            Assert.True(expected(actual));

            // {type}P =eq= value
            query = $"{obj1.GetType().Name}P=neq={obj1ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        protected virtual void OnShouldBeNotEqualsWithNullable()
        {
            var obj1 = Manifest1();
            var obj2 = Manifest2();
            var obj1ToString = Manifest1ToString();
            var query = $"{obj1.GetType().Name}NullP!={obj1ToString}";
            // {type}P == value
            var expected = BuildExpression(query);
            var actual = Actual(obj2, true);
            Assert.True(expected(actual));

            // {type}P =eq= value
            query = $"{obj1.GetType().Name}NullP=neq={obj1ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        #endregion

        #region LowerThan

        protected virtual void OnShouldBeLowerThan()
        {
            var obj1 = Manifest1();
            var obj2ToString = Manifest2ToString();
            var query = $"{obj1.GetType().Name}P<{obj2ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1, true);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj1.GetType().Name}P=lt={obj2ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        protected virtual void OnShouldBeLowerThanWithNullable()
        {
            var obj1 = Manifest1();
            var obj2ToString = Manifest2ToString();
            var query = $"{obj1.GetType().Name}NullP<{obj2ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1, true);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj1.GetType().Name}NullP=lt={obj2ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        #endregion

        #region LowerThanOrEquals

        protected virtual void OnShouldBeLowerThanOrEquals()
        {
            var obj1 = Manifest1();
            var obj2ToString = Manifest2ToString();
            var query = $"{obj1.GetType().Name}P<={obj2ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1, true);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj1.GetType().Name}P=le={obj2ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        protected virtual void OnShouldBeLowerThanOrEqualsWithNullable()
        {
            var obj1 = Manifest1();
            var obj2ToString = Manifest2ToString();
            var query = $"{obj1.GetType().Name}NullP<={obj2ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj1, true);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj1.GetType().Name}NullP=le={obj2ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        #endregion

        #region GreaterThan

        protected virtual void OnShouldBeGreaterThan()
        {
            var obj2 = Manifest2();
            var obj1ToString = Manifest1ToString();
            var query = $"{obj2.GetType().Name}P>{obj1ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj2);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj2.GetType().Name}P=gt={obj1ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        protected virtual void OnShouldBeGreaterThanWithNullable()
        {
            var obj2 = Manifest2();
            var obj1ToString = Manifest1ToString();
            var query = $"{obj2.GetType().Name}NullP>{obj1ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj2, true);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj2.GetType().Name}NullP=gt={obj1ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        #endregion

        #region GreaterThanOrEquals

        protected virtual void OnShouldBeGreaterThanOrEquals()
        {
            var obj2 = Manifest2();
            var obj1ToString = Manifest1ToString();
            var query = $"{obj2.GetType().Name}P>={obj1ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj2);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj2.GetType().Name}P=ge={obj1ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        protected virtual void OnShouldBeGreaterThanOrEqualsWithNullable()
        {
            var obj2 = Manifest2();
            var obj1ToString = Manifest1ToString();

            var query = $"{obj2.GetType().Name}NullP>={obj1ToString}";
            // {type}NUllP == value
            var expected = BuildExpression(query);
            var actual = Actual(obj2, true);
            Assert.True(expected(actual));

            // {type}NUllP =eq= value
            query = $"{obj2.GetType().Name}NullP=ge={obj1ToString}";
            expected = BuildExpression(query);
            Assert.True(expected(actual));
        }

        #endregion
    }
}
