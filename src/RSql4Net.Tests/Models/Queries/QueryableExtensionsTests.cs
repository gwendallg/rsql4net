using System;
using System.Linq;
using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using Xunit;
using RSql4Net.Models;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;

namespace RSql4Net.Tests.Models.Queries
{
    public class QueryableExtensionsTests
    {
        [Fact]
        public void ShouldBeThrowArgumentNullException()
        {
            var pageable = new RSqlPageable<Customer>(2, 10);
            var customers = new Faker<Customer>().Generate(0).AsQueryable();

            // ArgumentNullException : obj
            this.Invoking(f => { QueryableExtensions.Page(null, pageable); })
                .Should().Throw<ArgumentNullException>();
            
            // ArgumentNullException : pageable
            this.Invoking(f => { QueryableExtensions.Page(customers, null); })
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldBePageWithNoSort()
        {
            var id = 0;
            var customers = new Faker<Customer>()
                .CustomInstantiator(f=> new Customer(){Id = id++})
                .Generate(100).AsQueryable();
            var pageable = new RSqlPageable<Customer>(2, 10);
            var query = new RSqlQuery<Customer>(c => c.Id >= 0);
            var expected = QueryableExtensions.Page(customers,pageable, query);
            var first = customers.Skip(20).First();
            expected.Number
                .Should().Be(2);
            expected.HasPrevious
                .Should().BeTrue();
            expected.HasNext
                .Should().BeTrue();
            expected.TotalElements
                .Should().Be(customers.Count());
            expected.TotalPages
                .Should().Be(10);
            expected.Content.First()
                .Should().Be(first);
        }

        [Fact]
        public void ShouldBePageWithSortOrderBy()
        {
            var customers = new Faker<Customer>()
                .CustomInstantiator(f => new Customer() {Id = f.Random.Int(0, 100)})
                .Generate(100).AsQueryable();

            Expression<Func<Customer, object>> a = (c) => c.Id;
            var sort = new RSqlSort<Customer>(){Value = a};
            var pageable = new RSqlPageable<Customer>(2, 10, sort);
            var query = new RSqlQuery<Customer>(c => c.Id >= 0);

            var expected = QueryableExtensions.Page(customers, pageable, query);
            var first = customers.OrderBy(c => c.Id).Skip(20).First();
            expected.Number
                .Should().Be(2);
            expected.HasPrevious
                .Should().BeTrue();
            expected.HasNext
                .Should().BeTrue();
            expected.TotalElements
                .Should().Be(customers.Count());
            expected.TotalPages
                .Should().Be(10);
            expected.Content.First()
                .Should().Be(first);
        }

        [Fact]
        public void ShouldBePageWithSortOrderByDescending()
        {
            var customers = new Faker<Customer>()
                .CustomInstantiator(f=> new Customer(){Id = f.Random.Int(0,100)})
                .Generate(100).AsQueryable();
            
            Expression<Func<Customer, object>> a = (c) => c.Id;
            var sort = new RSqlSort<Customer>() {Value = a, IsDescending = true};
            var pageable = new RSqlPageable<Customer>(2,10, sort);

            var expected = QueryableExtensions.Page(customers, pageable);
            var first = customers.OrderByDescending(c=>c.Id).Skip(20).First();
            expected.Number
                .Should().Be(2);
            expected.HasPrevious
                .Should().BeTrue();
            expected.HasNext
                .Should().BeTrue();
            expected.TotalElements
                .Should().Be(customers.Count());
            expected.TotalPages
                .Should().Be(10);
            expected.Content.First()
                .Should().Be(first);
        }
    }
}
