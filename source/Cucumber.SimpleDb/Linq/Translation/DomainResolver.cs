using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;
using Cucumber.SimpleDb.Session;

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
            if (qex.Source is ConstantExpression)
            {
                var source = (ConstantExpression)qex.Source;
                if (source.Value is ISimpleDbItemCollection)
                {
                    return SimpleDbExpression.Query(
                        qex.Select,
                        SimpleDbExpression.Domain(((ISimpleDbItemCollection)source.Value).Domain),
                        qex.Where,
                        qex.OrderBy,
						qex.Limit);
                }
                else
                {
                    throw new NotSupportedException(
                        string.Format("Cannot determine data source from '{0}'", source.Value.GetType())
                        );
                }
            }
            else
            {
                Expression source = base.Visit(qex.Source);
                return SimpleDbExpression.Query(
                    qex.Select,
                    source,
                    qex.Where,
                    qex.OrderBy,
					qex.Limit);
            }
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            Expression qex = VisitSimpleDbQuery(pex.Source);
            return SimpleDbExpression.Project(
                (QueryExpression)qex,
                pex.Projector);
        }
    }
}
