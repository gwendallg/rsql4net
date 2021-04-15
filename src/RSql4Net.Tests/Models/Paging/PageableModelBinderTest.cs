using System;
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
            // constructor: settings is null
            this.Invoking((a) => _= new RSqlPageableModelBinder<Customer>(null, Helper.JsonOptions(), Helper.MockLogger<Customer>().Object))
                .Should()
                .Throw<ArgumentNullException>();
            
            // constructor: option is null
            this.Invoking((a) => _= new RSqlPageableModelBinder<Customer>(Helper.Settings(), null, Helper.MockLogger<Customer>().Object))
                .Should()
                .Throw<ArgumentNullException>();
            
            // constructor: logger is null
            this.Invoking((a) => _= new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions(), null))
                .Should()
                .Throw<ArgumentNullException>();
            
            // build
            this.Invoking((a) =>
                {
                    var binder =  new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions(), Helper.MockLogger<Customer>().Object);
                    binder.Build(null);
                })
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
           
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions(), Helper.MockLogger<Customer>().Object);
            await pageableModelBinder.BindModelAsync(mock);

            var expected = mock.Result.Model as IRSqlPageable<Customer>;

            expected.Should().NotBeNull();

            expected?.PageNumber()
                .Should().Be(2);

            expected?
                .PageSize().Should().Be(1);

            expected?
                .Sort().Should().NotBeNull();

            expected?
                .Sort()?.IsDescending.Should().BeTrue();

            expected?
                .Sort().Next.Should().NotBeNull();
                
            expected?
                .Sort()?.Next?.IsDescending.Should().BeFalse();

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
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions(), Helper.MockLogger<Customer>().Object);
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
            
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions(), Helper.MockLogger<Customer>().Object);
            var expected = pageableModelBinder.Build(queryCollection);

            // exp
            expected
                .PageNumber().Should().Be(2);
            expected
                .PageSize().Should().Be(1);

            // expected on sort order by ( first )
            expected.Sort()
                .Should().NotBeNull();

            // first sort
            expected.Sort().IsDescending
                .Should().BeTrue();
            
            ((MemberExpression)((UnaryExpression)expected.Sort().Value.Body).Operand).Member.Name
                .Should().Be("Name");

            // expected on sort order by ( second )
            expected.Sort().Next.Should()
                .NotBeNull();
            
            // second sort
            expected.Sort().Next.IsDescending
                .Should().BeFalse();
        
            ((MemberExpression)((UnaryExpression)expected.Sort().Next.Value.Body).Operand).Member.Name
                .Should().Be("BirthDate");
        }

        [Fact]
        public void ShouldBeValidPageNumber()
        {
            const int pageNumber = 10;
            var queryCollection = Helper.QueryCollection("pageNumber", Convert.ToString(pageNumber));
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions(), Helper.MockLogger<Customer>().Object);
            var expected = pageableModelBinder.Build(queryCollection);
            expected.PageNumber()
                .Should().Be(pageNumber);
        }

        [Fact]
        public void ShouldBeValidPageSize()
        {
            const int pageSize = 10;
            var queryCollection = Helper.QueryCollection("pageSize", Convert.ToString(pageSize));
            var pageableModelBinder = new RSqlPageableModelBinder<Customer>(Helper.Settings(), Helper.JsonOptions(),Helper.MockLogger<Customer>().Object);
            var expected = pageableModelBinder.Build(queryCollection);

            expected.PageSize()
                .Should().Be(pageSize);
        }
    }
}
