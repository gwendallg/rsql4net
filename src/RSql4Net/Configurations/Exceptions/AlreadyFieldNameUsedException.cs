using System;
using System.Runtime.Serialization;

namespace RSql4Net.Configurations.Exceptions
{
    [Serializable]
    public class AlreadyFieldNameUsedException : RSqlException
    {
        public AlreadyFieldNameUsedException(string fieldName, string value) : base(
            $"Field name {fieldName} already used by {value}")
        {
        }

        protected AlreadyFieldNameUsedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
