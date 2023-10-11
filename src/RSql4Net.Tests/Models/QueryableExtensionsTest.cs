using System;
using System.Linq;
using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using RSql4Net.Models;
using RSql4Net.Models.Paging;
using Xunit;

namespace RSql4Net.Tests.Models
{
    public class QueryableExtensionsTest
    {
        [Fact]
        public void ShouldBePageWithMultiSort()
        {
            var customerId = 1;
            var customerFaker = new Faker<Customer>()
                .CustomInstantiator(_ => new Customer {Id = customerId++})
                .RuleFor(o => o.BirthDate, f => f.Date.Past(20))
                .RuleFor(o => o.Company, f => f.Company.CompanyName())
                .RuleFor(o => o.Email, f => f.Internet.Email())
                .RuleFor(o => o.Name, f => f.Name.LastName())
                .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(o => o.Username, f => f.Internet.UserNameUnicode())
                .RuleFor(o => o.Website, f => f.Internet.Url());


            var parameter = Expression.Parameter(typeof(Customer), "c");
            var nameProperty = Expression.Property(parameter, "Name");
            var expression = Expression.Convert(nameProperty, typeof(object));
            var lambda = Expression.Lambda<Func<Customer, object>>(expression,parameter);

            var sort = new RSqlSort<Customer>() {Value = lambda, IsDescending = true};
            nameProperty = Expression.Property(parameter, "BirthDate");
            expression = Expression.Convert(nameProperty, typeof(object));
            lambda = Expression.Lambda<Func<Customer, object>>(expression,parameter);
            sort = new RSqlSort<Customer>() {Value = lambda, IsDescending = false, Next = sort};

            var obj = customerFaker.Generate(100);
            var pageable = new RSqlPageable<Customer>(0, 100, sort);

            var expected = QueryableExtensions.Page(obj.AsQueryable(), pageable);
            var actual = obj.AsQueryable().OrderBy(c => c.BirthDate).ThenByDescending(c => c.Name);
            expected.Content.First()
                .Should().Be(actual.First());
            
            expected.Content.Last()
                .Should().Be(actual.Last());

        }
    }
}
