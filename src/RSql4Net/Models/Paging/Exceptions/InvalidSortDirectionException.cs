using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Paging.Exceptions
{
    [Serializable]
    public class InvalidSortDirectionException : RSqlPageableException
    {
        public InvalidSortDirectionException(object sortDirection,
            Exception innerException = null) : base(
            $"Invalid sort direction : {sortDirection}", innerException)
        {
        }

        protected InvalidSortDirectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
