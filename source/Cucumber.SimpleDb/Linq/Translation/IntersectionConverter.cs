using System;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class IntersectionConverter : SimpleDbExpressionVisitor
    {
        public static Expression Convert(Expression ex)
        {
            return new IntersectionConverter().Visit(ex);
        }

        private AttributeExpression _carriedAttribute;

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method == typeof(SimpleDbAttributeValue).GetMethod ("Intersection"))
            {
                var attributeExpression = m.Object as AttributeExpression;
                var replacer = new ValueAttributeReplacer((AttributeExpression)m.Object);
                foreach(var predicate in ((NewArrayExpression)m.Arguments[0]).Expressions)
                {
                    var body = ((Expression<Func<SimpleDbAttributeValue, bool>>)predicate).Body;
                    body = replacer.Visit(body);
                }
                Expression.MakeBinary(
                    (ExpressionType)SimpleDbExpressionType.Intersection,
                    null, null);
            }
            return base.VisitMethodCall (m);
        }

        private class ValueAttributeReplacer : SimpleDbExpressionVisitor
        {
            private AttributeExpression _attributeExpression;

            public ValueAttributeReplacer(AttributeExpression attributeExpression)
            {
                _attributeExpression = attributeExpression;
            }

            protected override Expression VisitParameter (ParameterExpression node)
            {
                if (node.Type == typeof(SimpleDbAttributeValue))
                {
                    return _attributeExpression;
                }
                else
                {
                    return base.VisitParameter (node);
                }
            }
        }
    }
}

