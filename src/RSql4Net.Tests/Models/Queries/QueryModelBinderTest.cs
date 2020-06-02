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
            var logger = new Mock<ILogger<Customer>>();
            // constructor
            this.Invoking((a) =>
                {
                    new RSqlQueryModelBinder<Customer>(null, option, logger.Object);
                })
                .Should()
                .Throw<ArgumentNullException>();

            // constructor
            this.Invoking((a) =>
                {
                    new RSqlQueryModelBinder<Customer>(settings, null, logger.Object);
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
            var mockLogger = new Mock<ILogger<Customer>>();
            mockLogger.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            
            var queryModelBinder = new RSqlQueryModelBinder<Customer>(new Settings(), Helper.JsonOptions(), mockLogger.Object);
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
                QueryCache = new MemoryCache(new MemoryCacheOptions()),
                OnCreateCacheEntry = (m) => { m.Size = 1024;}
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

            settings.QueryCache.TryGetValue(query, out var expected2);
            expected.Should().Be(expected2);
        }
    }
}
