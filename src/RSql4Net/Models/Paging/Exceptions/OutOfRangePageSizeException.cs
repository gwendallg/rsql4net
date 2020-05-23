using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Paging.Exceptions
{
    [Serializable]
    public class OutOfRangePageSizeException : PageableException
    {
        public OutOfRangePageSizeException(object pageSize,
            Exception innerException = null) : base($"out of range page size  : {pageSize}",
            innerException)
        {
        }

        protected OutOfRangePageSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
