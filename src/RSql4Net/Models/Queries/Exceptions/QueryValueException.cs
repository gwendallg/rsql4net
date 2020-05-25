using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public abstract class QueryValueException : QueryException<RSqlQueryParser.ValueContext>
    {
        protected QueryValueException(RSqlQueryParser.ValueContext origin, string message, Exception innerException = null)
            : base(origin, message, innerException)
        {
        }

        protected QueryValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
