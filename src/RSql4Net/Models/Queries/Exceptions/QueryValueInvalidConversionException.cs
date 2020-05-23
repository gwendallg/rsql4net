using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class QueryValueInvalidConversionException : QueryValueException
    {
        public QueryValueInvalidConversionException(QueryParser.ValueContext origin, Type type,
            Exception innerException = null) : base(origin,
            string.Format("{0} is not convertible to {1}.{2}", origin?.GetText(), type?.Namespace, type?.Name),
            innerException)
        {
        }

        protected QueryValueInvalidConversionException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
