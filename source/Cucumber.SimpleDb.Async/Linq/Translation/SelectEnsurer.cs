using System.Linq;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Async.Linq.Structure;

namespace Cucumber.SimpleDb.Async.Linq.Translation
{
    internal sealed class SelectEnsurer : SimpleDbExpressionVisitor
    {
        public static Expression Ensure(Expression expr)
        {
            expr = new SelectEnsurer().Visit(expr);
            return expr;
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            return SimpleDbExpression.Project(
                (QueryExpression) Visit(pex.Source),
                pex.Projector);
        }

        protected override Expression VisitSimpleDbQuery(QueryExpression qex)
        {
            //if select list is explicit, must ensure attributes referenced in OrderBy are included
            if (qex.Select.Attributes.Any() && qex.OrderBy.Any())
            {
                return SimpleDbExpression.Query(
                    SimpleDbExpression.Select(
                        qex.Select.Attributes.Union(qex.OrderBy.Select(ob => ob.Attribute))
                        ),
                    qex.Source,
                    qex.Where,
                    qex.OrderBy,
                    qex.Limit,
                    qex.UseConsistency);
            }
            return base.VisitSimpleDbQuery(qex);
        }
    }
}