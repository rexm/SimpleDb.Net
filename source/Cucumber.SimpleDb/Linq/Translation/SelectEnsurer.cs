using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class SelectEnsurer : SimpleDbExpressionVisitor
    {
        public static Expression Ensure(Expression expr)
        {
            expr = new SelectEnsurer().Visit(expr);
            return expr;
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            return SimpleDbExpression.Project(
                (QueryExpression)Visit(pex.Source),
                pex.Projector);
        }

        protected override Expression VisitSimpleDbQuery(QueryExpression qex)
        {
            //if select list is explicit, must ensure attributes referenced in OrderBy are included
            if (qex.Select.Attributes.Count() > 0 && qex.OrderBy.Count() > 0)
            {
                return SimpleDbExpression.Query(
                    SimpleDbExpression.Select(
                        qex.Select.Attributes.Union(qex.OrderBy.Select(ob => ob.Attribute))
                    ),
                    qex.Source,
                    qex.Where,
                    qex.OrderBy,
					qex.Limit);
            }
            return base.VisitSimpleDbQuery(qex);
        }
    }
}
