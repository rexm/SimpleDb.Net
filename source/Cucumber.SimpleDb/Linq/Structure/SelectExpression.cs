using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal class SelectExpression : SimpleDbExpression
    {
        private readonly IEnumerable<AttributeExpression> _attributes;

        public IEnumerable<AttributeExpression> Attributes
        {
            get { return _attributes; }
        }

        public SelectExpression(IEnumerable<AttributeExpression> attributes)
        {
            _attributes = attributes;
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)SimpleDbExpressionType.Select;
            }
        }

        public override Type Type
        {
            get
            {
                return typeof(IEnumerable<ISimpleDbItem>);
            }
        }
    }
}
