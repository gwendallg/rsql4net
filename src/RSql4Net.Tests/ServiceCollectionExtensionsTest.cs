using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using RSql4Net.Configurations;
using Xunit;
using  FluentAssertions;

namespace RSql4Net.Tests
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        void ShouldBeWithRSql()
        {
            var serviceCollection = new ServiceCollection();
            var applicationPartManager = new ApplicationPartManager();
            var mvcBuilder = new MvcBuilder(serviceCollection, applicationPartManager);
            ServiceCollectionExtensions.AddRSql(mvcBuilder);

            var expected1 = serviceCollection
                    .SingleOrDefault(c => c.ServiceType == typeof(Settings));

            expected1.Should().NotBeNull();
            expected1.Lifetime
                .Should().Be(ServiceLifetime.Singleton);

          
        }
    }
}
