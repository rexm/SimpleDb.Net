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
            return Expression.Call(
                Expression.Constant(this),
                this.GetType().GetMethod("ExecuteDeferred", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(projector.Body.Type),
                Expression.Constant(new QueryCommand(pex.Source)),
                projector
            );
        }

        protected virtual IEnumerable<T> ExecuteDeferred<T>(QueryCommand query, Func<ISimpleDbItem, T> projector)
        {
            foreach (var item in HydrateItems(query))
            {
                yield return projector(item);
            }
        }

        protected virtual IEnumerable<ISimpleDbItem> HydrateItems(QueryCommand query)
        {
            XElement result = null;
            string nextPageToken = null;
            do
            {
                result = _context.Service.Select(query.QueryText, false, nextPageToken);
                foreach (var itemNode in result.Elements("Item"))
                {
                    yield return new SessionSimpleDbItem(_context, query.Domain, itemNode, query.ExplicitSelect);
                }
                nextPageToken = result.Elements("NextPageToken").Select(x => x.Value).FirstOrDefault();
            } while (!string.IsNullOrEmpty(nextPageToken));
        }
    }
}
