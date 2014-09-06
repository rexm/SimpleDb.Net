using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using Cucumber.SimpleDb.Linq.Structure;
using Cucumber.SimpleDb.Session;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class QueryProxyWriter : SimpleDbExpressionVisitor
    {
        private static readonly XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly IInternalContext _context;

        private QueryProxyWriter(IInternalContext context)
        {
            _context = context;
        }

        public static Expression Rewrite(Expression expr, IInternalContext context)
        {
            return new QueryProxyWriter(context).Visit(expr);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            return Expression.Call(
                m.Object,
                m.Method,
                m.Arguments.Select(Visit).ToArray());
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            pex = ClientProjectionWriter.Rewrite(pex);
            var projector = pex.Projector as LambdaExpression;
            if (pex.Source.Select is ScalarExpression)
            {
                return Expression.Call(
                    Expression.Constant(this),
                    this.GetType().GetMethod("ExecuteScalar", BindingFlags.NonPublic | BindingFlags.Instance)
                        .MakeGenericMethod(pex.Source.Select.Type),
                    Expression.Constant(new QueryCommand(pex.Source)));
            }
            else
            {
                return Expression.Call(
                    Expression.Constant(this),
                    this.GetType().GetMethod("ExecuteDeferred", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(projector.Body.Type),
                    Expression.Constant(new QueryCommand(pex.Source)),
                    projector);
            }
            return Expression.Call(
                Expression.Constant(this),
                GetType().GetMethod("ExecuteDeferred", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(projector.Body.Type),
                Expression.Constant(new QueryCommand(pex.Source)),
                projector);
        }

        protected virtual T ExecuteScalar<T>(QueryCommand query)
        {
            var result = ExecuteQuery(query).FirstOrDefault();
            if (result == null)
            {
                throw new InvalidOperationException("Query did not return a scalar value in the result");
            }
            return (T)Convert.ChangeType(result
                .Element(SdbNs + "Attribute")
                .Element(SdbNs + "Value").Value, typeof(T));
        }

        protected virtual IEnumerable<T> ExecuteDeferred<T>(QueryCommand query, Func<ISimpleDbItem, T> projector)
        {
            return ExecuteQuery(query).Select(itemData => projector(new SessionSimpleDbItem(_context, query.Domain, itemData, query.ExplicitSelect)));
        }

        protected virtual IEnumerable<XElement> ExecuteQuery(QueryCommand query)
        {
            string nextPageToken = null;
            do
            {
                var result = _context.Service.Select(query.QueryText, query.UseConsistency, nextPageToken)
                    .Element(SdbNs + "SelectResult");
                foreach (var itemNode in result.Elements(SdbNs + "Item"))
                {
                    yield return itemNode;
                }
                nextPageToken = result.Elements(SdbNs + "NextToken").Select(x => x.Value).FirstOrDefault();
            } while (!string.IsNullOrEmpty(nextPageToken));
        }
    }
}
