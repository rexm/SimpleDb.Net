using System.Linq;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;
using Cucumber.SimpleDb.Utilities;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class ItemAttributeMapper : SimpleDbExpressionVisitor
    {
        public static Expression Eval(Expression expression)
        {
            return new ItemAttributeMapper().Visit(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (typeof(ISimpleDbItem).IsAssignableFrom(node.Member.DeclaringType))
            {
                switch (node.Member.Name)
                {
                    case "Name":
                        return SimpleDbExpression.Function("itemName", typeof(string));
                }
            }
            return base.VisitMember(node);
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
            return Expression.New(
                n.Constructor,
                n.Arguments.Select(arg => this.Visit(arg)).ToArray());
        }

        private static AttributeExpression GenerateAttributeExpression(ConstantExpression expression)
        {
            return SimpleDbExpression.Attribute(expression.Value as string, typeof (SimpleDbAttributeValue));
        }
    }
}
