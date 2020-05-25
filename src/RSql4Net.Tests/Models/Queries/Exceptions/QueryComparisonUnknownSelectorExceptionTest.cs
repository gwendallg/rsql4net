using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public class CommonQueryComparisonUnknownSelectorExceptionTest : CommonQueryExceptionTest
    {
        [Fact]
        public void ShouldBeSerializable()
        {
            OnShouldBeSerializable<QueryComparisonUnknownSelectorException>();
        }
    }
}
