using FluentAssertions;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class QueryModelBinderProviderTest
    {

        [Fact]
        public void ShouldBeNotQueryModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(string));
            var expected = new QueryModelBinderProvider(new Settings());
            expected.GetBinder(modelBinderProviderContextMock)
                .Should().BeNull();
        }

        [Fact]
        public void ShouldBeQueryModelBinder()
        {
            var modelBinderProviderContextMock =
                new MockModelBinderProviderContext(typeof(Query<Customer>));
            var expected = new QueryModelBinderProvider(new Settings());
            expected.GetBinder(modelBinderProviderContextMock)
                .Should().NotBeNull();
        }
    }
}
