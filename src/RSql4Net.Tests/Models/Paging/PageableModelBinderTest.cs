using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using RSql4Net.Configurations;
using RSql4Net.Models.Paging;
using Xunit;

namespace RSql4Net.Tests.Models.Paging
{
    public class PageableModelBinderTest
    {

        [Fact]
        public async void ShouldBeBindModelAsyncTest()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("PageSize", new StringValues("1")),
                        new KeyValuePair<string, StringValues>("PageNumber", new StringValues("2")),
                        new KeyValuePair<string, StringValues>("Sort", new StringValues("Name;desc,BirthDate"))
                    }
                ));

            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };
            var pageableModelBinder = new PageableModelBinder<Customer>(new Settings());
            await pageableModelBinder.BindModelAsync(mock);

            var expected = mock.Result.Model as IPageable<Customer>;

            expected.Should().NotBeNull();

            expected
                .PageNumber().Should().Be(2);

            expected
                .PageSize().Should().Be(1);

            expected
                .Sort().Should().NotBeNull();

            mock.ModelState
                .IsValid.Should().BeTrue();
        }

        [Fact]
        public async void ShouldBeBindModelAsyncWithModelErrorTest()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("PageSize", new StringValues("a")),
                        new KeyValuePair<string, StringValues>("PageNumber", new StringValues("2")),
                        new KeyValuePair<string, StringValues>("Sort", new StringValues("Name;desc,BirthDate"))
                    }
                ));

            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };
            var pageableModelBinder = new PageableModelBinder<Customer>(new Settings());
            await pageableModelBinder.BindModelAsync(mock);

            var expected = mock.Result;
            expected.Model
                .Should().BeNull();

            mock.ModelState
                .IsValid.Should().BeFalse();
        }


        [Fact]
        public void ShouldBeIsValid()
        {
            var queryCollection = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("PageSize", new StringValues("1")),
                        new KeyValuePair<string, StringValues>("PageNumber", new StringValues("2")),
                        new KeyValuePair<string, StringValues>("Sort", new StringValues("Name;desc,BirthDate"))
                    }
                ));
            var pageableModelBinder = new PageableModelBinder<Customer>(new Settings());
            var expected = pageableModelBinder.Build(queryCollection);

            // exp
            expected
                .PageNumber().Should().Be(2);
            expected
                .PageSize().Should().Be(1);

            // expected on sort orderby
            var order = new List<Expression<Func<Customer, object>>>(expected.Sort().OrderBy);

            Assert.True(order.Count == 1);
            Assert.True(((MemberExpression)((UnaryExpression)order[0].Body).Operand).Member.Name == "BirthDate");

            order = new List<Expression<Func<Customer, object>>>(expected.Sort().OrderDescendingBy);
            Assert.True(((MemberExpression)((UnaryExpression)order[0].Body).Operand).Member.Name == "Name");
            Assert.True(order.Count == 1);
        }

        [Fact]
        public void ShouldBeValidPageNumber()
        {
            var pageNumber = 10;

            var queryColletion = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("PageNumber",
                            new StringValues(Convert.ToString(pageNumber)))
                    }
                ));
            var pageableModelBinder = new PageableModelBinder<Customer>(new Settings());
            var expected = pageableModelBinder.Build(queryColletion);

            expected.PageNumber()
                .Should().Be(pageNumber);
        }

        [Fact]
        public void ShouldBeValidPageSize()
        {
            var pageSize = 10;

            var queryColletion = new QueryCollection(
                new Dictionary<string, StringValues>(new[]
                    {
                        new KeyValuePair<string, StringValues>("PageSize", new StringValues(Convert.ToString(pageSize)))
                    }
                ));
            var pageableModelBinder = new PageableModelBinder<Customer>(new Settings());
            var expected = pageableModelBinder.Build(queryColletion);

            expected.PageSize()
                .Should().Be(pageSize);
        }
    }
}
