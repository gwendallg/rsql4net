using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class QueryModelBinderTest
    {

        [Fact]
        public void ShouldBeThrowArgumentNullException()
        {
            var option = Helper.JsonOptions();
            var settings = Helper.Settings();
            
            // constructor : settings is null
            this.Invoking((a) =>
                {
                   _=  new RSqlQueryModelBinder<Customer>(null, option, Helper.MockLogger<Customer>().Object);
                })
                .Should()
                .Throw<ArgumentNullException>();

            // constructor: options is null
            this.Invoking((a) =>
                {
                    _= new RSqlQueryModelBinder<Customer>(settings, null, Helper.MockLogger<Customer>().Object);
                })
                .Should()
                .Throw<ArgumentNullException>();
            
            // constructor: options is null
            this.Invoking((a) =>
                {
                    _= new RSqlQueryModelBinder<Customer>(settings, Helper.JsonOptions(), null);
                })
                .Should()
                .Throw<ArgumentNullException>();
            
            // build
            this.Invoking((a) =>
                {
                    var binder =  new RSqlQueryModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions(), Helper.MockLogger<Customer>().Object);
                    binder.Build(null);
                })
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public async void ShouldBeBindModelAsyncTest()
        {
            var queryCollection = Helper.QueryCollection("query", "name==a*");
            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };
            var queryModelBinder = new RSqlQueryModelBinder<Customer>(new Settings(), Helper.JsonOptions(), Helper.MockLogger<Customer>().Object);
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
            var queryCollection = Helper.QueryCollection("query", "name=a*");
            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };
            var mockLogger = new Mock<ILogger<Customer>>();
            mockLogger.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            var queryModelBinder = new RSqlQueryModelBinder<Customer>(new Settings(), Helper.JsonOptions(), mockLogger.Object);
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
            const string query = "name==a*";
            var queryCollection = Helper.QueryCollection("query", query);

            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };

            var settings = new Settings
            {
                QueryCache = new MemoryRSqlQueryCache(
                    new MemoryCacheOptions(),
                    onSet: (m) => { m.Size = 1024;}
                )
            };
            var mockLogger = new Mock<ILogger<Customer>>();
            mockLogger.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            
            var queryModelBinder = new RSqlQueryModelBinder<Customer>(settings, Helper.JsonOptions(),mockLogger.Object);
            await queryModelBinder.BindModelAsync(mock);

            var expected = mock.Result.Model as IRSqlQuery<Customer>;

            expected
                .Should().NotBeNull();

            mock.ModelState
                .IsValid.Should().BeTrue();

            settings.QueryCache.TryGetValue<Customer>(query, out var expected2);
            expected.Should().Be(expected2);
        }
    }
}
