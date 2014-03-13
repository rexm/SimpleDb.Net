using System;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Utilities;
using System.Linq;
using Cucumber.SimpleDb.Linq.Structure;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class ImplicitSelect : SimpleDbExpressionVisitor
    {
        public static Expression EnsureQuery(Expression exp)
        {
            return new ImplicitSelect ().Visit (exp);
        }

        private ImplicitSelect ()
        {
        }
            
        protected override Expression VisitConstant (ConstantExpression node)
        {
            if (node.Value is ISimpleDbItemCollection)
            {
                return SimpleDbExpression.Query (
                    SimpleDbExpression.Select (Enumerable.Empty<AttributeExpression> ()),
                    node,
                    null,
                    null,
                    null);
            }
            else
            {
                return base.VisitConstant (node);
            }
        }
    }
}

