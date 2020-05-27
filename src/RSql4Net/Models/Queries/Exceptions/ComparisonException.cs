using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public abstract class ComparisonException : QueryException<RSqlQueryParser.ComparisonContext>
    {
        protected ComparisonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected ComparisonException(RSqlQueryParser.ComparisonContext origin, string message,
            Exception innerException = null) : base(origin, message, innerException)
        {
        }
    }
}
