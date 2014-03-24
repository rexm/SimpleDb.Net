using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Async.Linq.Translation
{
    internal abstract class SimpleDbExpressionVisitor : ExpressionVisitor
    {
        public override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch ((SimpleDbExpressionType) exp.NodeType)
            {
                case SimpleDbExpressionType.Attribute:
                    return VisitSimpleDbAttribute((AttributeExpression) exp);
                case SimpleDbExpressionType.Query:
                    return VisitSimpleDbQuery((QueryExpression) exp);
                case SimpleDbExpressionType.Projection:
                    return VisitSimpleDbProjection((ProjectionExpression) exp);
                case SimpleDbExpressionType.Count:
                    return VisitSimpleDbCount((CountExpression) exp);
                case SimpleDbExpressionType.Select:
                    return VisitSimpleDbSelect((SelectExpression) exp);
                case SimpleDbExpressionType.Order:
                    return VisitSimpleDbOrder((OrderExpression) exp);
                case SimpleDbExpressionType.Domain:
                    return VisitSimpleDbDomain((DomainExpression) exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitSimpleDbDomain(DomainExpression dex)
        {
            return dex;
        }

        protected virtual Expression VisitSimpleDbOrder(OrderExpression oex)
        {
            return oex;
        }

        protected virtual Expression VisitSimpleDbSelect(SelectExpression sex)
        {
            return sex;
        }

        protected virtual Expression VisitSimpleDbCount(CountExpression cex)
        {
            return cex;
        }

        protected virtual Expression VisitSimpleDbQuery(QueryExpression qex)
        {
            return qex;
        }

        protected virtual Expression VisitSimpleDbAttribute(AttributeExpression nex)
        {
            return nex;
        }

        protected virtual Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            return pex;
        }
    }
}