using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class QueryComparisonInvalidComparatorSelectionException : QueryComparisonException
    {
        public QueryComparisonInvalidComparatorSelectionException(QueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Invalid selector : {origin?.selector()?.GetText()}", innerException)
        {
        }

        protected QueryComparisonInvalidComparatorSelectionException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
