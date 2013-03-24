// Based on the TypeHelper class by Matt Warren:
// http://iqtoolkit.codeplex.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Cucumber.SimpleDb.Utilities
{
    internal static class TypeUtilities
    {
        public static T Attribute<T>(this MemberInfo type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>().FirstOrDefault();
        }

        public static T[] Attributes<T>(this MemberInfo type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>().ToArray();
        }

        public static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
            {
                return null;
            }
            if (seqType.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType (seqType.GetElementType ());
            }
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                    {
                        return ienum;
                    }
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable (seqType.BaseType);
            }
            return null;
        }

        public static Type GetSequenceType(Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        public static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null)
            {
                return seqType;
            }
            return ienum.GetGenericArguments()[0];
        }

        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullAssignable(Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType (type))
            {
                return type.GetGenericArguments () [0];
            }
            return type;
        }

        public static Type GetMemberType(MemberInfo mi)
        {
            FieldInfo fi = mi as FieldInfo;
            if (fi != null)
            {
                return fi.FieldType;
            }
            PropertyInfo pi = mi as PropertyInfo;
            if (pi != null)
            {
                return pi.PropertyType;
            }
            EventInfo ei = mi as EventInfo;
            if (ei != null)
            {
                return ei.EventHandlerType;
            }
            return null;
        }

        //adapted from http://stackoverflow.com/questions/3631547/select-right-generic-method-with-reflection/9496602#9496602
        public static MethodInfo GetMethod(this Type type, string name, params Type[] parameters)
        {
            MethodInfo match = null;
            foreach (var method in type.GetMethods())
            {
                if (method.Name != name)
                {
                    continue;
                }
                if (method.IsGenericMethodDefinition)
                {
                    var substitutions = new Dictionary<Type, Type>();
                    var genericArguments = method.GetGenericArguments();
                    if (genericArguments.Length >= 1)
                    {
                        substitutions[typeof(Ref.T1)] = genericArguments[0];
                    }
                    if (genericArguments.Length >= 2)
                    {
                        substitutions[typeof(Ref.T2)] = genericArguments[1];
                    }
                    if (genericArguments.Length >= 3)
                    {
                        substitutions[typeof (Ref.T3)] = genericArguments[2];
                    }
                    if (genericArguments.Length > 3)
                    {
                        throw new NotSupportedException("Too many type parameters.");
                    }
                    match = TryMatch (parameters, method, t => Substitute(t, substitutions));
                }
                else
                {
                    match = TryMatch (parameters, method, t => t);
                }
                if(match != null)
                {
                    break;
                }
            }
            return match;
        }

        private static MethodInfo TryMatch (
            Type[] parameters,
            MethodInfo method,
            Func<Type, Type> getParameterType)
        {
            var methodParameters = method.GetParameters ();
            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameterTypeAtPosition = getParameterType(parameters[i]);
                if (parameterTypeAtPosition != methodParameters [i].ParameterType)
                {
                    return null;
                }
            }
            return method;
        }

        private static Type Substitute(Type type, IDictionary<Type, Type> substitutions)
        {
            if (type.IsGenericType)
            {
                var typeArgs = type.GetGenericArguments();
                for(int i = 0; i < typeArgs.Length; i++)
                {
                    typeArgs[i] = Substitute(typeArgs[i], substitutions);
                }
                type = type.GetGenericTypeDefinition();
                type = type.MakeGenericType(typeArgs);
            }
            return substitutions.ContainsKey(type) ? substitutions[type] : type;
        }
    }

    internal static class Ref
    {
        public sealed class T1 { }
        public sealed class T2 { }
        public sealed class T3 { }
    }
}
