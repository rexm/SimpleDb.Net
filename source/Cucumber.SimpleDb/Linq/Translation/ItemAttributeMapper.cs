﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        break;
                }
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method == typeof(ISimpleDbItem).GetMethod("get_Item"))
            {
                return GenerateAttributeExpression((ConstantExpression)m.Arguments[0].ReduceTotally());
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
            return Expression.New(
                n.Constructor,
                n.Arguments.Select(arg => this.Visit(arg)).ToArray());
        }

        private AttributeExpression GenerateAttributeExpression(ConstantExpression expression)
        {
            return SimpleDbExpression.Attribute(expression.Value as string, typeof(SimpleDbAttributeValue));
        }
    }
}
