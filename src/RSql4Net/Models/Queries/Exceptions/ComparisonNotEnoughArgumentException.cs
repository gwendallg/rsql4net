using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class ComparisonNotEnoughArgumentException : ComparisonException
    {
        public ComparisonNotEnoughArgumentException(RSqlQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Not enough argument : {origin?.selector()?.GetText()}", innerException)
        {
        }

        protected ComparisonNotEnoughArgumentException(SerializationInfo info, StreamingContext context) : base(
            info, context)
        {
        }
    }
}
