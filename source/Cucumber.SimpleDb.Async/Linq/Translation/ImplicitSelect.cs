using System.Linq;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Async.Linq.Structure;

namespace Cucumber.SimpleDb.Async.Linq.Translation
{
    internal sealed class ImplicitSelect : SimpleDbExpressionVisitor
    {
        private ImplicitSelect()
        {
        }

        public static Expression EnsureQuery(Expression exp)
        {
            return new ImplicitSelect().Visit(exp);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is ISimpleDbItemCollection)
            {
                return SimpleDbExpression.Query(
                    SimpleDbExpression.Select(Enumerable.Empty<AttributeExpression>()),
                    node,
                    null,
                    null,
                    null,
                    false);
            }
            return base.VisitConstant(node);
        }
    }
}