using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal class ProjectionExpression : SimpleDbExpression
    {
        private readonly QueryExpression _source;
        private readonly Expression _projector;
        private readonly Expression _originalExpression;

        public ProjectionExpression(QueryExpression source, Expression projector)
            : this(source, projector, null)
        {
        }

        public ProjectionExpression(QueryExpression source, Expression projector, Expression originalExpression)
        {
            this._source = source;
            this._projector = projector;
            this._originalExpression = originalExpression;
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
            get
            {
                return (ExpressionType)SimpleDbExpressionType.Projection;
            }
        }

        public override Type Type
        {
            get
            {
                if (_projector != null)
                {
                    return _projector.Type;
                }
                else
                {
                    return typeof(IEnumerable<ISimpleDbItem>);
                }
            }
        }
    }
}
