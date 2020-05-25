using System;
using System.Runtime.Serialization;

namespace RSql4Net
{
    [Serializable]
    public abstract class RSqlException : Exception
    {
        protected RSqlException(string message) : base(message)
        {
        }

        protected RSqlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RSqlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
