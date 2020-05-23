using System;
using System.Runtime.Serialization;

namespace RSql4Net.Models.Paging.Exceptions
{
    [Serializable]
    public class UnknownSortException : PageableException
    {
        public UnknownSortException(object sort,
            Exception innerException = null) : base($"Unknown sort : {sort}", innerException)
        {
        }

        protected UnknownSortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
