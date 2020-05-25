using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public class CommonQueryErrorNodeExceptionTest : CommonQueryExceptionTest
    {
        [Fact]
        public void ShouldBeSerializable()
        {
            var mockSelectorContext = new MockSelectorContext("A");
            var mockArgumentsContext = new MockArgumentsContext("A");
            var mockComparisonContext = new MockComparisonContext(mockSelectorContext, mockArgumentsContext);

            var actual = new QueryErrorNodeException(null);
            var fileName = Path.GetRandomFileName();
            using var stream = new FileStream(fileName, FileMode.Create);
            var formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            formatter.Serialize(stream, actual);
            stream.Position = 0;
            var expected = formatter.Deserialize(stream) as QueryErrorNodeException;

            expected
                .Should().NotBeNull();

            expected.Message
                .Should()
                .Equals(actual.Message);
        }
    }
}
