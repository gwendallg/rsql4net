using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace RSql4Net.Models
{
    internal static class EnumMemberReflectionHelper
    {
        private const BindingFlags EnumBindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
        
        private static readonly Dictionary<Type, Dictionary<string, object>> Mapping =
            new Dictionary<Type, Dictionary<string, object>>();

        public static bool TryParse(Type enumType, string value, out object result)
        {
            result = default;

            if (string.IsNullOrEmpty(value)) return false;
            
            var mapping = GetOrRegistryEnum(enumType);
            return mapping != null && mapping.TryGetValue(value, out result);
        }

        private static Dictionary<string, object> GetOrRegistryEnum(Type enumType)
        {
            lock (Mapping)
            {
                if (!Mapping.ContainsKey(enumType))
                {
                    Mapping[enumType] = Build(enumType);
                }

                return Mapping.TryGetValue(enumType, out var result) ? result : null;
            }
        }

        private static Dictionary<string, object> Build(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new ArgumentException("Type provided must be an Enum.", nameof(enumType));

            var result = new Dictionary<string, object>();
            var builtInNames = enumType.GetEnumNames();
            var builtInValues = enumType.GetEnumValues();

            for (var i = 0; i < builtInNames.Length; i++)
            {
                var field = enumType.GetField(builtInNames[i], EnumBindings)!;
                var enumMemberAttribute = field.GetCustomAttribute<EnumMemberAttribute>(true);

                if (!string.IsNullOrEmpty(enumMemberAttribute?.Value))
                {
                    result[enumMemberAttribute.Value] = builtInValues.GetValue(i);
                }
            }

            return result;
        }
    }
}
