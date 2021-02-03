using System;
using System.Collections.Generic;
using System.Reflection;

namespace RSql4Net.Models
{
    public class MethodContainsInfo
    {
        private readonly MethodInfo _addMethod;
        private readonly ConstructorInfo _constructor;

        public MethodContainsInfo(Type type)
        {
            var t = typeof(List<>).MakeGenericType(type);
            _constructor = t.GetConstructor(Type.EmptyTypes);
            _addMethod = t.GetMethod("Add", new[] {type});
            ContainsMethod = t.GetMethod("Contains", new[] {type});
        }

        public MethodInfo ContainsMethod { get; }

        public object Convert(List<object> values)
        {
            if (values == null)
            {
                return null;
            }

            var result = _constructor.Invoke(Array.Empty<object>());
            values.ForEach(a =>
            {
                _addMethod.Invoke(result, new[] {a});
            });
            return result;
        }
    }
}
