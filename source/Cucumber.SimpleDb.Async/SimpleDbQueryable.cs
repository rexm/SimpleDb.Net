using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cucumber.SimpleDb.Async
{
    /// <summary>
    /// Provides SimpleDb-specific IQueryable extensions
    /// </summary>
    public static class SimpleDbQueryable
    {
        /// <summary>
        /// Indicates the query should be executed using ConsistentRead=true
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/ConsistencySummary.html
        /// </summary>
        /// <returns>The same IQueryable with a ConsistentRead context.</returns>
        /// <param name="source">The IQueryable.</param>
        /// <typeparam name="TSource">The type of the query.</typeparam>
        public static IQueryable<TSource> WithConsistency<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.CreateQuery<TSource>(
                StaticCall(
                    MakeGeneric(MethodBase.GetCurrentMethod(), typeof (TSource)),
                    source.Expression));
        }

        private static MethodInfo MakeGeneric(MethodBase method, params Type[] parameters)
        {
            return ((MethodInfo) method).MakeGenericMethod(parameters);
        }

        private static Expression StaticCall(MethodInfo method, params Expression[] expressions)
        {
            return Expression.Call(null, method, expressions);
        }
    }
}