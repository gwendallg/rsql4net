using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RSql4Net.Models.Paging;
using Xunit;

namespace RSql4Net.Tests.Models.Paging
{
    public class PageableModelBinderTest
    {
        [Fact]
        public void ShouldBeThrowArgumentNullException()
        {
            this.Invoking((a) => new RSqlPageableModelBinder<Customer>(null, Helper.JsonOptions()))
                .Should()
                .Throw<ArgumentNullException>();
            
            this.Invoking((a) => new RSqlPageableModelBinder<Customer>(Helper.Settings(), null))
                .Should()
                .Throw<ArgumentNullException>();
        }
        
        [Fact]
        public async void ShouldBeBindModelAsyncTest()
        {
            var queryCollection = Helper.QueryCollection
            (
                "pageSize", "1",
                "pageNumber", "2",
                "sort", "name;desc,birthDate"
            );

            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };
           
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions());
            await pageableModelBinder.BindModelAsync(mock);

            var expected = mock.Result.Model as IRSqlPageable<Customer>;

            expected.Should().NotBeNull();

            expected.PageNumber()
                .Should().Be(2);

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
            var queryCollection = Helper.QueryCollection
            (
                "pageSize", "a",
                "pageNumber", "2",
                "sort", "name;desc,birthDate"
            );

            var context = new DefaultHttpContext();
            context.Request.Query = queryCollection;
            var actionContext = new ActionContext {HttpContext = context};
            var mock = new MockModelBindingContext
            {
                ActionContext = actionContext, ModelState = new ModelStateDictionary()
            };
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions());
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

            var queryCollection = Helper.QueryCollection
            (
                "pageSize", "1",
                "pageNumber", "2",
                "sort", "name;desc,birthDate"
            );
            
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions());
            var expected = pageableModelBinder.Build(queryCollection);

            // exp
            expected
                .PageNumber().Should().Be(2);
            expected
                .PageSize().Should().Be(1);

            // expected on sort order by
            var order = new List<Expression<Func<Customer, object>>>(expected.Sort().OrderBy);

            order.Count
                .Should().Be(1);

            ((MemberExpression)((UnaryExpression)order[0].Body).Operand).Member.Name
                .Should().Be("BirthDate");

            order = new List<Expression<Func<Customer, object>>>(expected.Sort().OrderDescendingBy);
            order.Count
                .Should().Be(1);
            ((MemberExpression)((UnaryExpression)order[0].Body).Operand).Member.Name
                .Should().Be("Name");
        }

        [Fact]
        public void ShouldBeValidPageNumber()
        {
            const int pageNumber = 10;
            var queryCollection = Helper.QueryCollection("pageNumber", Convert.ToString(pageNumber));
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions());
            var expected = pageableModelBinder.Build(queryCollection);
            expected.PageNumber()
                .Should().Be(pageNumber);
        }

        [Fact]
        public void ShouldBeValidPageSize()
        {
            const int pageSize = 10;
            var queryCollection = Helper.QueryCollection("pageSize", Convert.ToString(pageSize));
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions());
            var expected = pageableModelBinder.Build(queryCollection);

            expected.PageSize()
                .Should().Be(pageSize);
        }
    }
}
