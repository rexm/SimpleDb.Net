using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Async.Linq.Structure
{
    internal class ProjectionExpression : SimpleDbExpression
    {
        private readonly Expression _originalExpression;
        private readonly Expression _projector;
        private readonly QueryExpression _source;

        public ProjectionExpression(QueryExpression source, Expression projector)
            : this(source, projector, null)
        {
        }

        public ProjectionExpression(QueryExpression source, Expression projector, Expression originalExpression)
        {
            _source = source;
            _projector = projector;
            _originalExpression = originalExpression;
        }

        public QueryExpression Source
        {
            get { return _source; }
        }

        public Expression Projector
        {
            get { return _projector; }
        }

        public Expression OriginalExpression
        {
            get { return _originalExpression; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType) SimpleDbExpressionType.Projection; }
        }

        public override Type Type
        {
            get
            {
                return _projector != null 
                    ? _projector.Type 
                    : typeof (IEnumerable<ISimpleDbItem>);
            }
        }
    }
}