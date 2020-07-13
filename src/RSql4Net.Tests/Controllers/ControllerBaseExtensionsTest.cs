using System;
using System.Linq;
using System.Net;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RSql4Net.Controllers;
using RSql4Net.Models;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;
using RSql4Net.Tests.Models;
using Xunit;

namespace RSql4Net.Tests.Controllers
{
    public class ControllerBaseExtensionsTest
    {
        [Fact]
        public void ShouldBeThrowArgumentNullException()
        {
            var fakeCustomers = new Faker<Customer>()
                .Rules((f, o) => o.Name = f.Name.FullName())
                .Generate(100)
                .AsQueryable();

            var pageable = new RSqlPageable<Customer>(0, 10);
            var controller = new MockController();
            var query = new RSqlQuery<Customer>(c=>c.Name!=string.Empty);
            var page = fakeCustomers.Page(pageable, query);
            
            // controllerBase is null
            this.Invoking((o) =>
                    ControllerBaseExtensions.Page(
                        null,
                        page))
                .Should()
                .Throw<ArgumentNullException>();

            // page is null
            this.Invoking((o) =>
                    ControllerBaseExtensions.Page<Customer>(controller,
                        null))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldBePartialContent()
        {
            var fakeCustomers = new Faker<Customer>()
                .Rules((f, o) => o.Name = f.Name.FullName())
                .Generate(100)
                .AsQueryable();


            Helper.Expression<Customer>("name=is-null=false");
            var pageable = new RSqlPageable<Customer>(0, 10);
            var controller = new MockController();
            var page = fakeCustomers.Page(pageable);
            var expected = controller.Page(page);
            expected
                .Should().BeOfType<ObjectResult>();
            ((ObjectResult)expected).StatusCode
                .Should().Be((int)HttpStatusCode.PartialContent);
            ((ObjectResult)expected).Value.As<IRSqlPage<Customer>>()
                .Should().NotBeNull();
            ((IRSqlPage<Customer>)((ObjectResult)expected).Value).Content
                .Count.Should().Be(10);
            ((IRSqlPage<Customer>)((ObjectResult)expected).Value).HasNext
                .Should().BeTrue();
        }
        
        [Fact]
        public void ShouldBeAllContent()
        {
            var fakeCustomers = new Faker<Customer>()
                .Rules((f, o) => o.Name = f.Name.FullName())
                .Generate(100)
                .AsQueryable();
            var value = Helper.Expression<Customer>("name=is-null=false");
            var query = new RSqlQuery<Customer>(value);
            var pageable = new RSqlPageable<Customer>(0, 100);
            var controller = new MockController();
            var page = fakeCustomers.Page(pageable, query);  
            var expected = controller.Page(page);
            expected
                .Should().BeOfType<ObjectResult>();
            ((ObjectResult)expected).StatusCode
                .Should().Be((int)HttpStatusCode.OK);
            ((ObjectResult)expected).Value.As<IRSqlPage<Customer>>()
                .Should().NotBeNull();
            ((IRSqlPage<Customer>)((ObjectResult)expected).Value).Content
                .Count.Should().Be(fakeCustomers.Count());
            ((IRSqlPage<Customer>)((ObjectResult)expected).Value).HasNext
                .Should().BeFalse();
        }
    }
}
