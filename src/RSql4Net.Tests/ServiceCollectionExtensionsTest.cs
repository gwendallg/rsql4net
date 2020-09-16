using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using RSql4Net.Configurations;
using Xunit;
using  FluentAssertions;
using Moq;

namespace RSql4Net.Tests
{
    public class ServiceCollectionExtensionsTest
    {

        [Fact]
        void ShouldBeWithArgumentNullException()
        {
            this
                .Invoking((f) => ServiceCollectionExtensions.AddRSql(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        void ShouldBeWithRSql()
        {
            var serviceCollection = new ServiceCollection();
            var applicationPartManager = new ApplicationPartManager();
            var mock = new Mock<IMvcBuilder>();
            mock.SetupGet(m => m.Services).Returns(serviceCollection);
            ServiceCollectionExtensions.AddRSql(mock.Object);

            var expected1 = serviceCollection
                    .SingleOrDefault(c => c.ServiceType == typeof(Settings));

            expected1.Should().NotBeNull();
            expected1.Lifetime
                .Should().Be(ServiceLifetime.Singleton);

        }
    }
}
