using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using RSql4Net.Configurations.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Configuration.Exceptions
{
    public class AlreadyFieldNameUsedExceptionTest
    {
        [Fact]
        public void ShouldBeSerializable()
        {
            var actual = new AlreadyFieldNameUsedException("A", "B");
            var fileName = Path.GetRandomFileName();
            using var stream = new FileStream(fileName, FileMode.Create);
            var formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            formatter.Serialize(stream, actual);
            stream.Position = 0;
            var expected = formatter.Deserialize(stream) as AlreadyFieldNameUsedException;
            expected
                .Should().NotBeNull();

            expected.Message
                .Should()
                .Equals(actual.Message);
        }
    }
}
