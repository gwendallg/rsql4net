using System;
using System.Runtime.Serialization;
using Antlr4.Runtime.Tree;

namespace RSql4Net.Models.Queries.Exceptions
{
    [Serializable]
    public class QueryErrorNodeException : QueryException<IErrorNode>
    {
        public QueryErrorNodeException(IErrorNode origin,
            Exception innerException = null) : base(origin,
            $"Error parsing : {origin?.ToStringTree()}", innerException)
        {
        }

        protected QueryErrorNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
