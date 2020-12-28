using FluentAssertions;
using RSql4Net.Models;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class QueryReflectionHelperTest
    {
        [Fact]
        public void ShouldBeExcludeProperty()
        {
            QueryReflectionHelper.GetOrRegistryProperty(typeof(MockQuery), "excludeProperty")
                .Should()
                .BeNull();
        }

        [Fact]
        public void ShouldBeOverrideProperty()
        {
            var _ = QueryReflectionHelper.GetOrRegistryProperty(typeof(MockQuery), "test");
            var a = QueryReflectionHelper
                .MappingJson2PropertyInfo[QueryReflectionHelper.CDefaultNamingStrategy]
                [typeof(MockQuery)]
                ["Test"]
                .Should().NotBeNull();
        }
        
        [Fact]
        public void ShouldBeParentProperty()
        {
            var _ = QueryReflectionHelper.GetOrRegistryProperty(typeof(MockQuery), "StringParentP");
            var a = QueryReflectionHelper
                .MappingJson2PropertyInfo[QueryReflectionHelper.CDefaultNamingStrategy]
                [typeof(MockQuery)]
                ["StringParentP"]
                .Should().NotBeNull();
        }
    }
}
        
