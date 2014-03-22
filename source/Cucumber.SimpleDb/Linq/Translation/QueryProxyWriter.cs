using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;
using Cucumber.SimpleDb.Linq.Structure;
using System.Reflection;
using Cucumber.SimpleDb.Session;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class QueryProxyWriter :  SimpleDbExpressionVisitor
    {
        public static Expression Rewrite(Expression expr, IInternalContext context)
        {
            return new QueryProxyWriter(context).Visit(expr);
        }

        private readonly IInternalContext _context;

        private QueryProxyWriter(IInternalContext context)
        {
            _context = context;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            return Expression.Call(
                m.Object,
                m.Method,
                m.Arguments.Select(arg => Visit(arg)).ToArray());
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            pex = ClientProjectionWriter.Rewrite(pex);
            var projector = pex.Projector as LambdaExpression;
            if (pex.Source.Select is ScalarExpression)
            {
                return Expression.Call (
                    Expression.Constant (this),
                    this.GetType ().GetMethod ("ExecuteScalar", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod (pex.Source.Select.Type),
                    Expression.Constant (new QueryCommand (pex.Source)));
            }
            else
            {
                return Expression.Call (
                    Expression.Constant (this),
                    this.GetType ().GetMethod ("ExecuteDeferred", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod (projector.Body.Type),
                    Expression.Constant (new QueryCommand (pex.Source)),
                    projector);
            }
        }

        protected virtual T ExecuteScalar<T>(QueryCommand query)
        {
            var result = ExecuteQuery (query).FirstOrDefault ();
            if (result == null)
            {
                throw new InvalidOperationException ("Query did not return a scalar value in the result");
            }
            return (T)Convert.ChangeType (result
                .Element(sdbNs + "Attribute")
                .Element(sdbNs + "Value").Value, typeof(T));
        }

        protected virtual IEnumerable<T> ExecuteDeferred<T>(QueryCommand query, Func<ISimpleDbItem, T> projector)
        {
            foreach (var itemData in ExecuteQuery(query))
            {
                yield return projector(new SessionSimpleDbItem(_context, query.Domain, itemData, query.ExplicitSelect));
            }
        }

        protected virtual IEnumerable<XElement> ExecuteQuery(QueryCommand query)
        {
            XElement result = null;
            string nextPageToken = null;
            do
            {
                result = _context.Service.Select(query.QueryText, query.UseConsistency, nextPageToken)
                    .Element(sdbNs + "SelectResult");
                foreach (var itemNode in result.Elements(sdbNs + "Item"))
                {
                    yield return itemNode;
                }
                nextPageToken = result.Elements(sdbNs + "NextToken").Select(x => x.Value).FirstOrDefault();
            } while (!string.IsNullOrEmpty(nextPageToken));
        }

        private readonly static XNamespace sdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
    }
}
