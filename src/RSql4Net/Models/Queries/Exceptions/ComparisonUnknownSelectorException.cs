using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class ComparisonUnknownSelectorException : ComparisonException
    {
        public ComparisonUnknownSelectorException(RSqlQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Unknown selector : '{origin?.selector()?.GetText()}'", innerException)
        {
        }

        protected ComparisonUnknownSelectorException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
