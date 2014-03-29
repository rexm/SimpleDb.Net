using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cucumber.SimpleDb.Async.Linq.Structure;
using Cucumber.SimpleDb.Async.Session;

namespace Cucumber.SimpleDb.Async.Linq.Translation
{
    internal sealed class QueryProxyWriter : SimpleDbExpressionVisitor
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
            return Expression.Call(m.Object, m.Method, m.Arguments.Select(Visit).ToArray());
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            pex = ClientProjectionWriter.Rewrite(pex);
            var projector = pex.Projector as LambdaExpression;
            if (pex.Source.Select is ScalarExpression)
            {
                return Expression.Call(Expression.Constant(this), GetType().GetMethod("ExecuteScalarAsync", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(pex.Source.Select.Type), Expression.Constant(new QueryCommand(pex.Source)));
            }
            return Expression.Call(Expression.Constant(this), GetType().GetMethod("ExecuteDeferredAsync", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(projector.Body.Type), Expression.Constant(new QueryCommand(pex.Source)), projector);
        }

        internal async Task<T> ExecuteScalarAsync<T>(QueryCommand query)
        {
            var result = (await ExecuteQueryAsync(query).ConfigureAwait(false)).FirstOrDefault();
            if (result == null)
            {
                throw new InvalidOperationException("Query did not return a scalar value in the result");
            }
            return (T) Convert.ChangeType(result.Element(SdbNs + "Attribute").Element(SdbNs + "Value").Value, typeof (T));
        }

        internal async Task<IEnumerable<T>> ExecuteDeferredAsync<T>(QueryCommand query, Func<ISimpleDbItem, T> projector)
        {
            return (await ExecuteQueryAsync(query).ConfigureAwait(false)).Select(itemData => projector(new SessionSimpleDbItem(_context, query.Domain, itemData, query.ExplicitSelect)));
        }

        private async Task<IEnumerable<XElement>> ExecuteQueryAsync(QueryCommand query)
        {
            var itemNodes = new List<XElement>();
            string nextPageToken = null;

            do
            {
                var result = (await _context.Service.SelectAsync(query.QueryText, query.UseConsistency, nextPageToken).ConfigureAwait(false)).Element(SdbNs + "SelectResult");

                itemNodes.AddRange(result.Elements(SdbNs + "Item"));

                nextPageToken = result.Elements(SdbNs + "NextToken").Select(x => x.Value).FirstOrDefault();

            } while (!string.IsNullOrEmpty(nextPageToken));

            return itemNodes;
        }
    }
}