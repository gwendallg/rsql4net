using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class QueryValueInvalidConversionException : QueryValueException
    {
        public QueryValueInvalidConversionException(RSqlQueryParser.ValueContext origin, Type type,
            Exception innerException = null) : base(origin,
            $"{origin?.GetText()} is not convertible to {type?.Namespace}.{type?.Name}",
            innerException)
        {
        }

        protected QueryValueInvalidConversionException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
