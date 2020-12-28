using System.IO;
using System.Runtime.Serialization;
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
            var serializer = new DataContractSerializer(typeof(AlreadyFieldNameUsedException));
            serializer.WriteObject(stream, actual);
            stream.Position = 0;
            var expected = serializer.ReadObject(stream) as AlreadyFieldNameUsedException;
            expected
                .Should().NotBeNull();
            expected?.Message
                .Should()
                .Be(actual.Message);
        }
    }
}
