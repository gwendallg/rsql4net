using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Paging.Exceptions
{
    [Serializable]
    public class InvalidPageNumberValueException : RSqlPageableException
    {
        public InvalidPageNumberValueException(object pageNumber,
            Exception innerException = null) : base(
            $"Invalid page number value : {pageNumber}", innerException)
        {
        }

        protected InvalidPageNumberValueException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
