using System.IO;
using System.Runtime.Serialization;
using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public class QueryErrorNodeExceptionTest : QueryExceptionTest
    {
        [Fact]
        public void ShouldBeSerializable()
        {
            //var mockSelectorContext = new MockSelectorContext("A");
            //var mockArgumentsContext = new MockArgumentsContext("A");
            //var mockComparisonContext = new MockComparisonContext(mockSelectorContext, mockArgumentsContext);

            var actual = new QueryErrorNodeException(null);
            var fileName = Path.GetRandomFileName();
            using var stream = new FileStream(fileName, FileMode.Create);
            var serializer = new DataContractSerializer(typeof(QueryErrorNodeException));
            serializer.WriteObject(stream, actual);
            stream.Position = 0;
            var expected = serializer.ReadObject(stream) as QueryErrorNodeException;

            expected
                .Should().NotBeNull();

            expected?.Message
                .Should()
                .Be(actual.Message);
        }
    }
}
