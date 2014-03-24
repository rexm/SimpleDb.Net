using System;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class DomainResolver : SimpleDbExpressionVisitor
    {
        public static Expression Resolve(Expression expr)
        {
            expr = new DomainResolver().Visit(expr);
            return expr;
        }

        protected override Expression VisitSimpleDbQuery(QueryExpression qex)
        {
            var constantExpression = qex.Source as ConstantExpression;
            if (constantExpression != null)
            {
                var source = constantExpression;
                var simpleDbItemCollection = source.Value as ISimpleDbItemCollection;
                if (simpleDbItemCollection != null)
                {
                    return SimpleDbExpression.Query(
                        qex.Select,
                        SimpleDbExpression.Domain(simpleDbItemCollection.Domain),
                        qex.Where,
                        qex.OrderBy,
                        qex.Limit,
                        qex.UseConsistency);
                }
                throw new NotSupportedException(
                    string.Format("Cannot determine data source from '{0}'", source.Value.GetType())
                    );
            }
            else
            {
                var source = base.Visit(qex.Source);
                return SimpleDbExpression.Query(
                    qex.Select,
                    source,
                    qex.Where,
                    qex.OrderBy,
                    qex.Limit,
                    qex.UseConsistency);
            }
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            var qex = VisitSimpleDbQuery(pex.Source);
            return SimpleDbExpression.Project(
                (QueryExpression) qex,
                pex.Projector);
        }
    }
}