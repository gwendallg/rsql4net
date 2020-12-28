using System;
using System.IO;
using System.Runtime.Serialization;
using FluentAssertions;
using RSql4Net.Models.Paging.Exceptions;

namespace RSql4Net.Tests.Models.Paging.Exceptions
{
    public class PageableExceptionTest<T> where T : RSqlPageableException 
    {
        protected void OnShouldBeSerializable() 
        {
            var constructor = typeof(T).GetConstructor(new[] {typeof(object), typeof(Exception)});
            var actual = (T)constructor?.Invoke(new object[] {null, null});
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
                .Be(actual?.Message);
        }
    }
}
