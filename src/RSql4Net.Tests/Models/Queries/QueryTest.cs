using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;

namespace RSql4Net.Tests.Models.Queries
{
    public abstract class QueryTest
    {
        protected QueryTest()
        {
            Settings = new Settings {QueryField = "q"};
            QueryModelBinder = new QueryModelBinder<MockQuery>(Settings);
        }

        protected Settings Settings { get; }
        protected QueryModelBinder<MockQuery> QueryModelBinder { get; }

        protected Expression<Func<MockQuery, bool>> BuildExpression(string query)
        {
            var dic = new Dictionary<string, StringValues> {[Settings.QueryField] = query};
            return QueryModelBinder
                .Build(new QueryCollection(dic))
                .Value();
        }

        protected Func<MockQuery, bool> BuildFunction(string query)
        {
            return BuildExpression(query).Compile();
        }
    }
}
