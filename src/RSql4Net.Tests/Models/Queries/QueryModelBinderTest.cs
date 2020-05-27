using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class QueryModelBinderTest
    {
        
        [Fact]
        public async void ShouldBeBindModelAsyncTest()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("Query", new StringValues("Name==a*"))
                    }
                ));

            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };
            var options = Options.Create(new JsonOptions()
            {
                JsonSerializerOptions = {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
            });
            var queryModelBinder = new RSqlQueryModelBinder<Customer>(new Settings(), options);
            await queryModelBinder.BindModelAsync(mock);

            var expected = mock.Result.Model as IRSqlQuery<Customer>;

            expected
                .Should().NotBeNull();

            mock.ModelState
                .IsValid.Should().BeTrue();
        }

        [Fact]
        public async void ShouldBeBindModelWithModelErrorAsyncTest()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("query", new StringValues("name=a*"))
                    }
                ));

            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };

            var options = Options.Create(new JsonOptions()
            {
                JsonSerializerOptions = {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
            });
            var queryModelBinder = new RSqlQueryModelBinder<Customer>(new Settings(), options);
            await queryModelBinder.BindModelAsync(mock);

            var expected = mock.Result;

            expected.Model
                .Should().BeNull();

            mock.ModelState
                .IsValid.Should().BeFalse();
        }
        
        [Fact]
        public async void ShouldBeBindModelAsyncWithCacheTest()
        {
            string query = "name==a*";
            
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("query", new StringValues(query))
                    }
                ));

            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };

            var settings = new Settings
            {
                QueryCache = new MemoryCache(new MemoryCacheOptions()),
                OnCreateCacheEntry = (m) => { m.Size = 1024;}
            };

            var options = Options.Create(new JsonOptions()
            {
                JsonSerializerOptions = {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
            });
            
            var queryModelBinder = new RSqlQueryModelBinder<Customer>(settings, options);
            await queryModelBinder.BindModelAsync(mock);

            var expected = mock.Result.Model as IRSqlQuery<Customer>;

            expected
                .Should().NotBeNull();

            mock.ModelState
                .IsValid.Should().BeTrue();

            settings.QueryCache.TryGetValue(query, out var expected2);
            expected.Should().Be(expected2);
        }
    }
}
