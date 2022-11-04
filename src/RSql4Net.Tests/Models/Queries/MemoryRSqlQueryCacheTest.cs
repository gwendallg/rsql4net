
using FluentAssertions;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class MemoryRSqlQueryCacheTest
    {
        [Fact]
        public void ShouldBeTryGetValueWithValue()
        {
            var actual = new MemoryRSqlQueryCache();
            const string key = "company==m*;id=lt=1000";
            var e = Helper.Expression<Customer>(key);
            var q = new RSqlQuery<Customer>(e);
            actual.Set(key, q);
            var expected = actual.TryGetValue<Customer>(key, out var result);
            expected.Should().BeTrue();
            result.Should().NotBeNull();
        }
        
        [Fact]
        public void ShouldBeTryGetValueWithNotValue()
        {
            var actual = new MemoryRSqlQueryCache();
            var expected = actual.TryGetValue<Customer>("test", out var result);
            expected.Should().BeFalse();
        }
    }
}
