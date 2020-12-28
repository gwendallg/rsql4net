using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using RSql4Net.Models.Queries;

namespace RSql4Net.Tests.Models.Queries.Exceptions
{
    public abstract class QueryExceptionTest 
    {
        protected void OnShouldBeSerializable<T>() where T : Exception
        {
            var mockSelectorContext = new MockSelectorContext("A");
            var mockArgumentsContext = new MockArgumentsContext("A");
            var mockComparisonContext = new MockComparisonContext(mockSelectorContext, mockArgumentsContext);
            var constructor = typeof(T).GetConstructor(new[] {typeof(RSqlQueryParser.ComparisonContext), typeof(Exception)});

            var actual = (T)constructor.Invoke(new object[] {mockComparisonContext, null});
            var fileName = Path.GetRandomFileName();
            using var stream = new FileStream(fileName, FileMode.Create);
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(stream, actual);
            stream.Position = 0;
            var expected = (T)serializer.ReadObject(stream);

            expected
                .Should().NotBeNull();

            expected?.Message
                .Should()
                .Be(actual.Message);
        }
    }
}
