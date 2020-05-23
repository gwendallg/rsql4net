using System;
using System.Runtime.Serialization;

namespace RSql4Net
{
    [Serializable]
    public abstract class RSql4NetException : Exception
    {
        protected RSql4NetException(string message) : base(message)
        {
        }

        protected RSql4NetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RSql4NetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
