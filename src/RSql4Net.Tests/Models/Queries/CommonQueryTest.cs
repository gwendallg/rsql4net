using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;

namespace RSql4Net.Tests.Models.Queries
{
    public abstract class CommonQueryTest
    {
        protected CommonQueryTest()
        {
            Settings = new Settings {QueryField = "q"};
            RSqlQueryModelBinder = new RSqlQueryModelBinder<MockQuery>(Settings);
        }

        protected Settings Settings { get; }
        protected RSqlQueryModelBinder<MockQuery> RSqlQueryModelBinder { get; }

        protected Expression<Func<MockQuery, bool>> BuildExpression(string query)
        {
            var dic = new Dictionary<string, StringValues> {[Settings.QueryField] = query};
            return RSqlQueryModelBinder
                .Build(new QueryCollection(dic))
                .Value();
        }

        protected Func<MockQuery, bool> BuildFunction(string query)
        {
            return BuildExpression(query).Compile();
        }
    }
}
