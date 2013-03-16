using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;
using Cucumber.SimpleDb.Utilities;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class IndexedAttributeMapper : SimpleDbExpressionVisitor
    {
        public static Expression Eval(Expression expression)
        {
            return new IndexedAttributeMapper().Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Type == typeof(SimpleDbAttributeValue))
            {
                if (m.Method.Name == "get_Item")
                {
                    return GenerateAttributeExpression((ConstantExpression)m.Arguments[0].ReduceTotally());
                }
            }
            return base.VisitMethodCall(m);
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            if (operand != u.Operand)
            {
                AttributeExpression attributeExpression = operand as AttributeExpression;
                if (attributeExpression != null && attributeExpression.Type == typeof(SimpleDbAttributeValue))
                {
                    return SimpleDbExpression.Attribute(attributeExpression.Name, u.Type);
                }
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return base.VisitUnary(u);
        }

        protected override Expression VisitNew(NewExpression n)
        {
            var replacedArguments = new List<Expression>();
            foreach (var argument in n.Arguments)
            {
                replacedArguments.Add(this.Visit(argument));
            }
            return Expression.New(n.Constructor, replacedArguments.ToArray());
        }

        private AttributeExpression GenerateAttributeExpression(ConstantExpression expression)
        {
            return new AttributeExpression(expression.Value as string, typeof(SimpleDbAttributeValue));
        }
    }
}
