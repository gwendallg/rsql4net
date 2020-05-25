using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public abstract class QueryException<T> : RSqlException
    {
        protected QueryException(T origin, string message, Exception innerException = null) : base(message,
            innerException)
        {
            Origin = origin;
        }

        protected QueryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public T Origin { get; }
    }
}
