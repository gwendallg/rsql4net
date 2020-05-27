using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class InvalidConversionException : QueryValueException
    {
        public InvalidConversionException(RSqlQueryParser.ValueContext origin, Type type,
            Exception innerException = null) : base(origin,
            $"{origin?.GetText()} is not convertible to {type?.Namespace}.{type?.Name}",
            innerException)
        {
        }

        protected InvalidConversionException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
