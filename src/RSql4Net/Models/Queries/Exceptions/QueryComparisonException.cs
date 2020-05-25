using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public abstract class QueryComparisonException : QueryException<RSqlQueryParser.ComparisonContext>
    {
        protected QueryComparisonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected QueryComparisonException(RSqlQueryParser.ComparisonContext origin, string message,
            Exception innerException = null) : base(origin, message, innerException)
        {
        }
    }
}
