using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class ComparisonInvalidMatchTypeException : ComparisonException
    {
        public ComparisonInvalidMatchTypeException(RSqlQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            $"Invalid comparison match type : {origin?.selector()?.GetText()} and {origin?.arguments()?.GetText()}",
            innerException)
        {
        }

        protected ComparisonInvalidMatchTypeException(SerializationInfo info, StreamingContext context) : base(
            info, context)
        {
        }
    }
}
