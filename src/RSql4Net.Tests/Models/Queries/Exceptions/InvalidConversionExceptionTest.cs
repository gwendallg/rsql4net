using System.IO;
using System.Runtime.Serialization;
using FluentAssertions;
using RSql4Net.Models.Queries.Exceptions;
using Xunit;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public class InvalidConversionExceptionTest
    {
        [Fact]
        public void ShouldBeSerializable()
        {
            var mockSelectorContext = new MockSelectorContext("A");
            var mockValueContext = new MockValueContext(mockSelectorContext,0);
          
            var actual = new InvalidConversionException(mockValueContext, null);
            var fileName = Path.GetRandomFileName();
            using var stream = new FileStream(fileName, FileMode.Create);
            var serializer = new DataContractSerializer(typeof(InvalidConversionException));
            serializer.WriteObject(stream, actual);
            stream.Position = 0;
            var expected = serializer.ReadObject(stream) as InvalidConversionException;

            expected
                .Should().NotBeNull();

            expected?.Message
                .Should()
                .Be(actual.Message);
        }

        [Fact]
        public void ShouldBeThrow()
        {
            const string query = "int32P==a";
            this
                .Invoking(_ => Helper.Expression<MockQuery>(query))
                .Should()
                .Throw<InvalidConversionException>()
                .And
                .Origin.Should()
                .NotBeNull();
        }
    }
}
