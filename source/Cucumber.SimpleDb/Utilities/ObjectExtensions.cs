using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Xml.Linq;
using System.Collections;

namespace Cucumber.SimpleDb.Utilities
{
    internal static class ObjectExtensions
    {
        public static TValue Try<TInput, TValue>(this TInput input, Func<TInput, TValue> value)
        {
            if (input != null)
            {
                return value(input);
            }
            return default(TValue);
        }

        public static string ToSafeString(this object value)
        {
            return (value ?? string.Empty).ToString();
        }

        public static XElement ToXElement(this object value, string name)
        {
            var children = new List<XElement>();
            if (typeof(IEnumerable).IsAssignableFrom(value.GetType()) || value.GetType().IsArray)
            {
                foreach (object childItem in (value as IEnumerable))
                {
                    children.Add(childItem.ToXElement(childItem.GetType().Name));
                }
            }
            else
            {
                foreach (var property in value.GetType().GetProperties())
                {
                    object childValue = property.GetValue(value, null);
                    if (property.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }
                    if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType)
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

        private class HasMemberTester : DynamicObject
        {
            private object instance;
            public HasMemberTester(object instance)
            {
                this.instance = instance;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                bool found = this.instance.GetType().GetMember(binder.Name) != null;
                result = found;
                return found;
            }
        }
            
    }
}
