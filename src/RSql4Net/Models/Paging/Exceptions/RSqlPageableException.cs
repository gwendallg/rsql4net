using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Paging.Exceptions
{
    [Serializable]
    public abstract class RSqlPageableException : RSqlException
    {
        protected RSqlPageableException(string message, Exception innerException = null) : base(
            message,
            innerException)
        {
        }

        protected RSqlPageableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
