using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Serialization;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class NamingStrategyTest
    {
        private Expression<Func<MockQuery, bool>> BuildExpression(Settings settings, string query)
        {
            var queryModelBinder = new RSqlQueryModelBinder<MockQuery>(settings);
            var dic = new Dictionary<string, StringValues> {[settings.QueryField] = query};
            return queryModelBinder
                .Build(new QueryCollection(dic))
                .Value();
        }


        [Fact]
        public void ShouldBeEqualsCamelCaseNamingStrategy()
        {
            var settings = new Settings(new CamelCaseNamingStrategy()) {QueryField = "q"};
            // ==
            var actual = "Param_0 => (Param_0.StringP == \"a\")";
            var query = "stringP==a";
            var expected = BuildExpression(settings, query).ToString();
            expected
                .Should().Equals(actual);
        }

        [Fact]
        public void ShouldBeEqualsPascalCaseNamingStrategy()
        {
            var settings = new Settings {QueryField = "q"};
            // ==
            var actual = "Param_0 => (Param_0.StringP == \"a\")";
            var query = "StringP==a";
            var expected = BuildExpression(settings, query).ToString();
            expected
                .Should().Equals(actual);
        }

        [Fact]
        public void ShouldBeEqualsSnakeCaseNamingStrategy()
        {
            var settings = new Settings(new SnakeCaseNamingStrategy()) {QueryField = "q"};
            // ==
            var actual = "Param_0 => (Param_0.StringP == \"a\")";
            var query = "string_p==a";
            var expected = BuildExpression(settings, query).ToString();
            expected
                .Should().Equals(actual);
        }
    }
}
