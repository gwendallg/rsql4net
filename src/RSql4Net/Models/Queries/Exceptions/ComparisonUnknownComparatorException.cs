using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class ComparisonUnknownComparatorException : ComparisonException
    {
        public ComparisonUnknownComparatorException(RSqlQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Unknown comparator : {origin?.comparator()?.GetText()}", innerException)
        {
        }

        protected ComparisonUnknownComparatorException(SerializationInfo info, StreamingContext context) : base(
            info, context)
        {
        }
    }
}
