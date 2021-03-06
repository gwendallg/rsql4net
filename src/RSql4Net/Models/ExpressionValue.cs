﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace RSql4Net.Models
{
    public class ExpressionValue
    {
        public PropertyInfo Property { get; private set; }
        public Expression Expression { get; private set; }

        public static bool TryParse<T>(ParameterExpression parameter,
            string selector, JsonNamingPolicy jsonNamingPolicy, out ExpressionValue result
        )
        {
            result = null;
            try
            {
                result = Parse<T>(parameter, selector, jsonNamingPolicy);
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        public static ExpressionValue Parse<T>(ParameterExpression parameter,
            string selector,
            JsonNamingPolicy jsonNamingPolicy)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (selector == null || string.IsNullOrEmpty(selector))
            {
                throw new ArgumentNullException(nameof(selector));
            }

            Expression lastMember = parameter;
            PropertyInfo property = null;
            var type = typeof(T);
            if (selector.IndexOf(".", StringComparison.InvariantCulture) != -1)
            {
                foreach (var item in selector.Split('.'))
                {
                    property = QueryReflectionHelper.GetOrRegistryProperty(type, item, jsonNamingPolicy);
                    if (property == null)
                    {
                        return null;
                    }

                    type = property.PropertyType;
                    lastMember = Expression.Property(lastMember, property);
                }
            }
            else
            {
                property = QueryReflectionHelper.GetOrRegistryProperty(type, selector, jsonNamingPolicy);
                if (property == null)
                {
                    return null;
                }

                lastMember = Expression.Property(lastMember, property);
            }

            return new ExpressionValue {Property = property, Expression = lastMember};
        }
    }
}
