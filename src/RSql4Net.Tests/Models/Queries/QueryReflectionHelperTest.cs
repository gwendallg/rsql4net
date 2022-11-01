using FluentAssertions;
using RSql4Net.Models;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class QueryReflectionHelperTest
    {
        [Theory]
        [InlineData(nameof(MockQuery.ExcludeProperty))]
        [InlineData(nameof(MockQuery.ExcludeProperty2))]
        public void ShouldBeExcludeProperty(string name)
        {
            QueryReflectionHelper.GetOrRegistryProperty(typeof(MockQuery), name)
                .Should()
                .BeNull();
        }

        [Theory]
        [InlineData("Test")]
        [InlineData("Test2")]
        public void ShouldBeOverrideProperty(string name)
        {
            var _ = QueryReflectionHelper.GetOrRegistryProperty(typeof(MockQuery), name);
            var a = QueryReflectionHelper.GetDefaultMappingForType(typeof(MockQuery))
                [name]
                .Should()
                .NotBeNull();
        }

        [Fact]
        public void ShouldBeParentProperty()
        {
            var _ = QueryReflectionHelper.GetOrRegistryProperty(typeof(MockQuery), "StringParentP");
            var a = QueryReflectionHelper.GetDefaultMappingForType(typeof(MockQuery))
                ["StringParentP"]
                .Should().NotBeNull();
        }
    }
}
        
