using System.Collections.Generic;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Async.Linq.Structure;

namespace Cucumber.SimpleDb.Async.Linq.Translation
{
    internal sealed class SelectionCollector : SimpleDbExpressionVisitor
    {
        private readonly List<AttributeExpression> _attributes = new List<AttributeExpression>();

        public static IEnumerable<AttributeExpression> Collect(Expression expr)
        {
            var projector = new SelectionCollector();
            projector.Visit(expr);
            return projector._attributes;
        }

        protected override Expression VisitSimpleDbAttribute(AttributeExpression aex)
        {
            _attributes.Add(aex);
            return base.VisitSimpleDbAttribute(aex);
        }
    }
}