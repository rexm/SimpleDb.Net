using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Linq.Structure;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class SelectionCollector : SimpleDbExpressionVisitor
    {
        public static IEnumerable<AttributeExpression> Collect(Expression expr)
        {
            var projector = new SelectionCollector();
            projector.Visit(expr);
            return projector._attributes;
        }

        private List<AttributeExpression> _attributes = new List<AttributeExpression>();

        protected override Expression VisitSimpleDbAttribute(AttributeExpression aex)
        {
            _attributes.Add(aex);
            return base.VisitSimpleDbAttribute(aex);
        }
    }
}
