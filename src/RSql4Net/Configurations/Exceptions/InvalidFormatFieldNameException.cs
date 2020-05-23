using System;
using System.Runtime.Serialization;

namespace RSql4Net.Configurations.Exceptions
{
    [Serializable]
    public class InvalidFormatFieldNameException : RSql4NetException
    {
        public InvalidFormatFieldNameException(string fieldName, string value) : base(
            $"Field identifier {value} : {fieldName} does not respect the expected format")
        {
        }

        protected InvalidFormatFieldNameException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
