using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class QueryComparisonNotEnoughArgumentException : QueryComparisonException
    {
        public QueryComparisonNotEnoughArgumentException(RSqlQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Not enough argument : {origin?.selector()?.GetText()}", innerException)
        {
        }

        protected QueryComparisonNotEnoughArgumentException(SerializationInfo info, StreamingContext context) : base(
            info, context)
        {
        }
    }
}
