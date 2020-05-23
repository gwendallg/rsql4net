using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using RSql4Net.Models.Paging.Exceptions;

namespace RSql4Net.Tests.Models.Paging.Exceptions
{
    public class PageableExceptionTest<T> where T : PageableException 
    {
        protected void OnShouldBeSerializable() 
        {
            var constructor = typeof(T).GetConstructor(new[] {typeof(object), typeof(Exception)});
            var actual = (T)constructor?.Invoke(new object[] {null, null});
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
