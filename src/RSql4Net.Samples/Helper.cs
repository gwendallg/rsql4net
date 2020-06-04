using System.Collections.Generic;
using Bogus;
using RSql4Net.Samples.Models;

namespace RSql4Net.Samples
{
    public static class Helper
    {
        public static IList<Customer> Fake(int count = 1000)
        {
            var customerId = 1;
            var addressFaker = new Faker<Address>()
                .RuleFor(o => o.City, f => f.Address.City())
                .RuleFor(o => o.Street, f => f.Address.StreetAddress())
                .RuleFor(o => o.Country, f => f.Address.Country())
                .RuleFor(o => o.Zipcode, f => f.Address.ZipCode());

            var customerFaker = new Faker<Customer>()
                .CustomInstantiator(f => new Customer {Id = customerId++})
                .RuleFor(o => o.Address, addressFaker.Generate())
                .RuleFor(o => o.BirthDate, f => f.Date.Past(20))
                .RuleFor(o => o.Company, f => f.Company.CompanyName())
                .RuleFor(o => o.Credit, f => f.Random.Double())
                .RuleFor(o => o.Debit, f => f.Random.Double())
                .RuleFor(o => o.Email, f => f.Internet.Email())
                .RuleFor(o => o.Name, f => f.Name.LastName())
                .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(o => o.Username, f => f.Internet.UserNameUnicode())
                .RuleFor(o => o.Website, f => f.Internet.Url());

            return customerFaker.Generate(count);
        }

    }
}
