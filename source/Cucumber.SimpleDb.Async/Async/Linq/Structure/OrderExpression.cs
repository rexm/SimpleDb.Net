using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Async.Linq.Structure
{
    internal class OrderExpression : SimpleDbExpression, IEqualityComparer<OrderExpression>
    {
        private readonly AttributeExpression _attribute;
        private readonly SortDirection _direction;

        public OrderExpression(AttributeExpression attribute, SortDirection direction)
        {
            _attribute = attribute;
            _direction = direction;
        }

        public SortDirection Direction
        {
            get { return _direction; }
        }

        public AttributeExpression Attribute
        {
            get { return _attribute; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType) SimpleDbExpressionType.Order; }
        }

        public override Type Type
        {
            get { return typeof (SimpleDbAttributeValue); }
        }

        public bool Equals(OrderExpression x, OrderExpression y)
        {
            if (x != null && y != null)
            {
                return x._attribute == y._attribute;
            }
            return x == y;
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