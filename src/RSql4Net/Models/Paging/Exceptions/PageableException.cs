using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Paging.Exceptions
{
    [Serializable]
    public abstract class PageableException : RSql4NetException
    {
        protected PageableException(string message, Exception innerException = null) : base(
            message,
            innerException)
        {
        }

        protected PageableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
