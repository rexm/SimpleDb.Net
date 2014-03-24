using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Async.Utilities
{
    internal static class ObjectExtensions
    {
        public static TValue Try<TInput, TValue>(this TInput input, Func<TInput, TValue> value)
            where TInput : class
        {
            return input != null ? value(input) : default(TValue);
        }

        public static string ToSafeString(this object value)
        {
            return (value ?? string.Empty).ToString();
        }

        public static XElement ToXElement(this object value, string name)
        {
            var children = new List<XElement>();
            if (value is IEnumerable || value.GetType().IsArray)
            {
                children.AddRange(from object childItem in (value as IEnumerable)
                    select childItem.ToXElement(childItem.GetType().Name));
            }
            else
            {
                foreach (var property in value.GetType().GetProperties())
                {
                    var childValue = property.GetValue(value, null);
                    if (property.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }
                    if (property.PropertyType == typeof (string) || property.PropertyType.IsValueType)
                    {
                        children.Add(new XElement(property.Name, childValue.ToString()));
                    }
                    else
                    {
                        children.Add(childValue.ToXElement(property.Name));
                    }
                }
            }
            return new XElement(name, children);
        }

        public static bool HasMember(this object item, string memberName)
        {
            return item.GetType().GetMember(memberName).Length > 0;
        }
    }
}