using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Paging.Exceptions
{
    [Serializable]
    public class InvalidPageSizeValueException : PageableException
    {
        public InvalidPageSizeValueException(object pageSize,
            Exception innerException = null) : base(
            $"Invalid page size value : {pageSize}", innerException)
        {
        }

        protected InvalidPageSizeValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
