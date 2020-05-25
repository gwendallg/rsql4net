using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public class CommonQueryComparisonNotEnoughArgumentExceptionTest : CommonQueryExceptionTest
    {
        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable<QueryComparisonNotEnoughArgumentException>();
        }
    }
}
