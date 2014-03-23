// Based on the TypeHelper class by Matt Warren:
// http://iqtoolkit.codeplex.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cucumber.SimpleDb.Utilities
{
    internal static class TypeUtilities
    {
        public static T Attribute<T>(this MemberInfo type)
        {
            return type.GetCustomAttributes(typeof (T), true).OfType<T>().FirstOrDefault();
        }

        public static T[] Attributes<T>(this MemberInfo type)
        {
            return type.GetCustomAttributes(typeof (T), true).OfType<T>().ToArray();
        }

        public static bool IsSubclassOfGenericType(Type genericType, Type subclass)
        {
            while (subclass != null && subclass != typeof (object))
            {
                var current = subclass.IsGenericType ? subclass.GetGenericTypeDefinition() : subclass;
                if (genericType == current)
                {
                    return true;
                }
                subclass = subclass.BaseType;
            }
            return false;
        }

        public static Type FindIEnumerable(Type seqType)
        {
            while (true)
            {
                if (seqType == null || seqType == typeof (string))
                {
                    return null;
                }
                if (seqType.IsArray)
                {
                    return typeof (IEnumerable<>).MakeGenericType(seqType.GetElementType());
                }
                if (seqType.IsGenericType)
                {
                    foreach (var arg in seqType.GetGenericArguments())
                    {
                        var ienum = typeof (IEnumerable<>).MakeGenericType(arg);
                        if (ienum.IsAssignableFrom(seqType))
                        {
                            return ienum;
                        }
                    }
                }
                var ifaces = seqType.GetInterfaces();
                if (ifaces != null && ifaces.Length > 0)
                {
                    foreach (var iface in ifaces)
                    {
                        var ienum = FindIEnumerable(iface);
                        if (ienum != null)
                        {
                            return ienum;
                        }
                    }
                }
                if (seqType.BaseType != null && seqType.BaseType != typeof (object))
                {
                    seqType = seqType.BaseType;
                    continue;
                }
                return null;
            }
        }

        public static Type GetSequenceType(Type elementType)
        {
            return typeof (IEnumerable<>).MakeGenericType(elementType);
        }

        public static Type GetElementType(Type seqType)
        {
            var ienum = FindIEnumerable(seqType);
            if (ienum == null)
            {
                return seqType;
            }
            return ienum.GetGenericArguments()[0];
        }

        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        public static bool IsNullAssignable(Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static Type GetMemberType(MemberInfo mi)
        {
            var fi = mi as FieldInfo;
            if (fi != null)
            {
                return fi.FieldType;
            }
            var pi = mi as PropertyInfo;
            if (pi != null)
            {
                return pi.PropertyType;
            }
            var ei = mi as EventInfo;
            if (ei != null)
            {
                return ei.EventHandlerType;
            }
            return null;
        }
    }
}