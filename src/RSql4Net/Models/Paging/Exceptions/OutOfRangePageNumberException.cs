using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Paging.Exceptions
{
    [Serializable]
    public class OutOfRangePageNumberException : RSqlPageableException
    {
        public OutOfRangePageNumberException(object pageNumber,
            Exception innerException = null) : base(
            $"Out of range page number  : {pageNumber}", innerException)
        {
        }

        protected OutOfRangePageNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
