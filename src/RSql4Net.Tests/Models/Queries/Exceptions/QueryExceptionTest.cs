using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using static RSql4Net.Models.Queries.QueryParser;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public abstract class QueryExceptionTest : QueryTest
    {
        protected void OnShouldBeSerializable<T>() where T : Exception
        {
            var mockSelectorContext = new MockSelectorContext("A");
            var mockArgumentsContext = new MockArgumentsContext("A");
            var mockComparisonContext = new MockComparisonContext(mockSelectorContext, mockArgumentsContext);
            var constructor = typeof(T).GetConstructor(new[] {typeof(ComparisonContext), typeof(Exception)});

            var actual = (T)constructor.Invoke(new object[] {mockComparisonContext, null});
            var fileName = Path.GetRandomFileName();
            using var stream = new FileStream(fileName, FileMode.Create);
            var formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            formatter.Serialize(stream, actual);
            stream.Position = 0;
            var expected = (T)formatter.Deserialize(stream);

            expected
                .Should().NotBeNull();

            expected.Message
                .Should()
                .Equals(actual.Message);
        }
    }
}
