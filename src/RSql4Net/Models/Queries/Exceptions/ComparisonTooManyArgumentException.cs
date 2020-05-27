using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class ComparisonTooManyArgumentException : ComparisonException
    {
        public ComparisonTooManyArgumentException(RSqlQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin, $"Too many arguments : {origin?.selector()?.GetText()}",
            innerException)
        {
        }

        protected ComparisonTooManyArgumentException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
