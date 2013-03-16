using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal class OrderExpression : SimpleDbExpression, IEqualityComparer<OrderExpression>
    {
        public SortDirection Direction { get { return _direction; } }

        public AttributeExpression Attribute { get { return _attribute; } }

        private readonly SortDirection _direction;
        private readonly AttributeExpression _attribute;

        public OrderExpression(AttributeExpression attribute, SortDirection direction)
        {
            _attribute = attribute;
            _direction = direction;
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)SimpleDbExpressionType.Order;
            }
        }

        public override Type Type
        {
            get
            {
                return typeof(SimpleDbAttributeValue);
            }
        }

        public bool Equals(OrderExpression x, OrderExpression y)
        {
            if (x != null && y != null)
            {
                return x._attribute == y._attribute;
            }
            else
            {
                return (object)x == (object)y;
            }
        }

        public int GetHashCode(OrderExpression obj)
        {
            return obj._attribute.GetHashCode();
        }
    }

    internal enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}
