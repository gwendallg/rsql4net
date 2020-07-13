using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;
using RSql4Net.Samples.Controllers;
using RSql4Net.Samples.Models;
using Xunit;

namespace RSql4Net.Samples.Tests.Controllers
{
    public class CustomerControllerTest
    {
        private static CustomerController SeTup(IList<Customer> data)
        {
            return  new CustomerController(data);
        }
        
        [Fact]
        public void ShouldBeGetPartialResult()
        {
            var data = Helper.Fake();
            var controller = SeTup(data);
            Expression<Func<Customer, bool>> value = customer => customer.Id >= 0;
            var actual = data.Where(value.Compile()).Skip(9).Take(9).ToList();
            var rSqlQuery = new RSqlQuery<Customer>(value);
            var rSqlPageable = new RSqlPageable<Customer>(1, 9);
            var expected = controller.Get(rSqlQuery, rSqlPageable);
            // is ObjectResult 
            expected
                .Should().BeOfType<ObjectResult>();
            // is partial result
            ((ObjectResult)expected).StatusCode
                .Should().Be((int)HttpStatusCode.PartialContent);
            // value is RSqlPage
            ((ObjectResult)expected).Value
                .As<IRSqlPage<Customer>>()
                .Should().NotBeNull();
            // is valid RSqlPage : first element
            ((RSqlPage<Customer>)((ObjectResult)expected).Value).Content.First()
                .Should().Be(actual.First());
            // is valid RSqlPage : last element
            ((RSqlPage<Customer>)((ObjectResult)expected).Value).Content.Last()
                .Should().Be(actual.Last());
            // is valid RSqlPage : HasNext
            ((RSqlPage<Customer>)((ObjectResult)expected).Value)
                .HasNext.Should().BeTrue();
            // is valid RSqlPage : HasPrevious
            ((RSqlPage<Customer>)((ObjectResult)expected).Value)
                .HasPrevious.Should().BeTrue();
        }

        [Fact]
        public void ShouldBeGetBadResult()
        {
            var data = Helper.Fake(10);
            var controller = SeTup(data);
            const string message1 = "Error 1";
            const string message2 = "Error 2";
            controller.ModelState.AddModelError("1",message1);
            controller.ModelState.AddModelError("2", message2);
            
            Expression<Func<Customer, bool>> value = customer => customer.Id > 0;
            var rSqlPageable = new RSqlPageable<Customer>(0, 10);
            var rSqlQuery = new RSqlQuery<Customer>(value);
            var expected = controller.Get(rSqlQuery, rSqlPageable);
            
            // is BadRequestResult 
            expected
                .Should().BeOfType<BadRequestObjectResult>();
            // is bad request status
            ((BadRequestObjectResult)expected).StatusCode
                .Should().Be((int)HttpStatusCode.BadRequest);
            // is Error Model
            ((BadRequestObjectResult)expected).Value
                .Should().BeOfType<ErrorModel>();
            
            // contains 2 messages
            ((ErrorModel)((BadRequestObjectResult)expected).Value).Messages.Count
                .Should().Be(2);
            // contains message 1
            ((ErrorModel)((BadRequestObjectResult)expected).Value).Messages
                .Should().Contain(message1);
            
            // contains message 2
            ((ErrorModel)((BadRequestObjectResult)expected).Value).Messages
                .Should().Contain(message2);
        }
        
        [Fact]
        public void ShouldBeGetOkResult()
        {
            var data = Helper.Fake(10);
            var controller = SeTup(data);
            Expression<Func<Customer, bool>> value = customer => customer.Id > 0;
            var actual = data.ToList();
            var rSqlQuery = new RSqlQuery<Customer>(value);
            var rSqlPageable = new RSqlPageable<Customer>(0, 10);
            var expected = controller.Get(rSqlQuery, rSqlPageable);
            // is ObjectResult 
            expected
                .Should().BeOfType<ObjectResult>();
            // is partial result
            ((ObjectResult)expected).StatusCode
                .Should().Be((int)HttpStatusCode.OK);
            // value is RSqlPage
            ((ObjectResult)expected).Value
                .As<IRSqlPage<Customer>>()
                .Should().NotBeNull();
            // is valid RSqlPage : first element
            ((RSqlPage<Customer>)((ObjectResult)expected).Value).Content.First()
                .Should().Be(actual.First());
            // is valid RSqlPage : last element
            ((RSqlPage<Customer>)((ObjectResult)expected).Value).Content.Last()
                .Should().Be(actual.Last());
            // is valid RSqlPage : HasNext
            ((RSqlPage<Customer>)((ObjectResult)expected).Value)
                .HasNext.Should().BeFalse();
            // is valid RSqlPage : HasPrevious
            ((RSqlPage<Customer>)((ObjectResult)expected).Value)
                .HasPrevious.Should().BeFalse();
        }
    }
}
