using System.Linq;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Async.Linq.Structure;
using Cucumber.SimpleDb.Async.Utilities;

namespace Cucumber.SimpleDb.Async.Linq.Translation
{
    internal class IndexedAttributeMapper : SimpleDbExpressionVisitor
    {
        public static Expression Eval(Expression expression)
        {
            return new IndexedAttributeMapper().Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            return m.Method == typeof (ISimpleDbItem).GetMethod("get_Item") 
                ? GenerateAttributeExpression((ConstantExpression) m.Arguments[0].ReduceTotally()) 
                : base.VisitMethodCall(m);
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            var operand = Visit(u.Operand);
            if (operand != u.Operand)
            {
                var attributeExpression = operand as AttributeExpression;
                if (attributeExpression != null && attributeExpression.Type == typeof (SimpleDbAttributeValue))
                {
                    return SimpleDbExpression.Attribute(attributeExpression.Name, u.Type);
                }
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return base.VisitUnary(u);
        }

        protected override Expression VisitNew(NewExpression n)
        {
            return Expression.New(n.Constructor, n.Arguments.Select(Visit).ToArray());
        }

        private static AttributeExpression GenerateAttributeExpression(ConstantExpression expression)
        {
            return SimpleDbExpression.Attribute(expression.Value as string, typeof (SimpleDbAttributeValue));
        }
    }
}