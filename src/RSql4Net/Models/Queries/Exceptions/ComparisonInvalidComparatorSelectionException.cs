using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class ComparisonInvalidComparatorSelectionException : ComparisonException
    {
        public ComparisonInvalidComparatorSelectionException(RSqlQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Invalid selector : {origin?.selector()?.GetText()}", innerException)
        {
        }

        protected ComparisonInvalidComparatorSelectionException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
