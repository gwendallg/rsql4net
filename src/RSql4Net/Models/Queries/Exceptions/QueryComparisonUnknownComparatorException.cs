using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class QueryComparisonUnknownComparatorException : QueryComparisonException
    {
        public QueryComparisonUnknownComparatorException(QueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Unknown comparator : {origin?.comparator()?.GetText()}", innerException)
        {
        }

        protected QueryComparisonUnknownComparatorException(SerializationInfo info, StreamingContext context) : base(
            info, context)
        {
        }
    }
}
