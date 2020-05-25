using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class QueryComparisonUnknownSelectorException : QueryComparisonException
    {
        public QueryComparisonUnknownSelectorException(RSqlQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Unknown selector : '{origin?.selector()?.GetText()}'", innerException)
        {
        }

        protected QueryComparisonUnknownSelectorException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
