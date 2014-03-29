// Portions copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See MicrosoftOpenTechnologiesLicense.txt in the project root for license information.
// This class has been modified from the original version at http://goo.gl/irW401.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Cucumber.SimpleDb.Async.Infrastructure;

namespace Cucumber.SimpleDb.Async.Utilities
{
    /// <summary>
    /// Provides SimpleDb-specific IQueryable extensions
    /// </summary>
    public static class SimpleDbQueryableExtensions
    {
        private static readonly MethodInfo CountMethod = GetMethod("Count", T => new[]
        {
            typeof (IQueryable<>).MakeGenericType(T)
        });

        /// <summary>
        /// Enumerates the asynchronous query results and performs the specified action on each element.
        /// </summary>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable{T}"/> to enumerate.</param>
        /// <param name="action">The action to perform on each element. </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public static async Task ForEachAsync<TSource>(this IQueryable<TSource> source, Action<TSource> action)
        {
            Check.NotNull(source, "source");
            Check.NotNull(action, "action");

            var enumerable = await AsEnumerableAsync(source).GetEnumerableAsync().ConfigureAwait(false);

            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// Asynchronously creates a <see cref="List{T}" /> from an <see cref="IQueryable{T}" />
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the data source.</typeparam>
        /// <param name="source">The data source.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="List{T}" /> that contains elements from the data source.
        /// </returns>
        public static async Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source)
        {
            Check.NotNull(source, "source");

            var list = new List<TSource>();
            await source.ForEachAsync(list.Add).ConfigureAwait(false);
            return list;
        }

        /// <summary>
        /// Asynchronously creates an array from an <see cref="IQueryable{T}" />
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the data source.</typeparam>
        /// <param name="source">The data source.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains an array that contains elements from the data source.
        /// </returns>
        public static async Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source)
        {
            Check.NotNull(source, "source");

            return (await source.ToListAsync().ConfigureAwait(false)).ToArray();
        }

        /// <summary>
        /// Asynchronously returns the number of elements in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.</param>
        /// <returns>
        /// A task result that may contain the number of elements in the source.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
        /// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
        public static async Task<int> CountAsync<TSource>(this IQueryable<TSource> source)
        {
            Check.NotNull(source, "source");

            return await source
                .GetAsyncQueryProvider()
                .ExecuteScalarAsync<int>(StaticCall(CountMethod, source))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Indicates the query should be executed using ConsistentRead=true
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/ConsistencySummary.html
        /// </summary>
        /// <returns>The same IQueryable with a ConsistentRead context.</returns>
        /// <param name="source">The IQueryable.</param>
        /// <typeparam name="TSource">The type of the query.</typeparam>
        public static IQueryable<TSource> WithConsistency<TSource>(this IQueryable<TSource> source)
        {
            Check.NotNull(source, "source");

            return source
                .Provider
                .CreateQuery<TSource>(StaticCall(MethodBase.GetCurrentMethod(), source));
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static Expression StaticCall<TSource>(MethodBase method, IQueryable<TSource> source)
        {
            return method.MakeGeneric(typeof(TSource)).StaticCall(source.Expression);
        }

        private static MethodInfo MakeGeneric(this MethodBase method, params Type[] parameters)
        {
            return ((MethodInfo) method).MakeGenericMethod(parameters);
        }

        private static Expression StaticCall(this MethodInfo method, params Expression[] expressions)
        {
            return Expression.Call(null, method, expressions);
        }

        private static IEnumerableAsync<T> AsEnumerableAsync<T>(this IQueryable<T> source)
        {
            Check.NotNull(source, "source");

            var enumerable = source as IEnumerableAsync<T>;

            if (enumerable != null)
            {
                return enumerable;
            }
            throw new SimpleDbException(string.Format("The source must be IEnumerableAsync<{0}>", typeof (T)));
        }

        private static IAsyncQueryProvider GetAsyncQueryProvider(this IQueryable source)
        {
            Check.NotNull(source, "source");

            var asyncQueryProvider = source.Provider as IAsyncQueryProvider;

            if (asyncQueryProvider != null)
            {
                return asyncQueryProvider;
            }
            throw new SimpleDbException("The provider must be IAsyncQueryProvider");
        }

        private static MethodInfo GetMethod(string methodName, Func<Type, Type[]> getParameterTypes)
        {
            return GetMethod(methodName, getParameterTypes.Method, 1);
        }

        private static MethodInfo GetMethod(string methodName, MethodInfo getParameterTypesMethod, int genericArgumentsCount)
        {
            var candidates = typeof (Queryable).GetTypeInfo().GetDeclaredMethods(methodName);

            foreach (var candidate in candidates)
            {
                var genericArguments = candidate.GetGenericArguments();
                if (genericArguments.Length == genericArgumentsCount && Matches(candidate, (Type[]) getParameterTypesMethod.Invoke(null, genericArguments)))
                {
                    return candidate;
                }
            }

            throw new SimpleDbException(String.Format("Method '{0}' with parameters '{1}' not found", methodName, PrettyPrint(getParameterTypesMethod, genericArgumentsCount)));
        }

        private static bool Matches(MethodInfo methodInfo, IEnumerable<Type> parameterTypes)
        {
            return methodInfo.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes);
        }

        private static string PrettyPrint(MethodInfo getParameterTypesMethod, int genericArgumentsCount)
        {
            var dummyTypes = new Type[genericArgumentsCount];
            for (var i = 0; i < genericArgumentsCount; i++)
            {
                dummyTypes[i] = typeof (object);
            }

            var parameterTypes = (Type[]) getParameterTypesMethod.Invoke(null, dummyTypes);
            var textRepresentations = new string[parameterTypes.Length];

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                textRepresentations[i] = parameterTypes[i].ToString();
            }

            return "(" + string.Join(", ", textRepresentations) + ")";
        }
    }
}