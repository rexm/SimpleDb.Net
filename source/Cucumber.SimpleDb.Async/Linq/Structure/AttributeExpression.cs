using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Async.Linq.Structure
{
    [DebuggerDisplay("`{Name}`")]
    internal class AttributeExpression : SimpleDbExpression, IEqualityComparer<AttributeExpression>
    {
        private readonly string _name;
        private readonly Type _type;

        public AttributeExpression(string name, Type type)
        {
            _name = name;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public override Type Type
        {
            get { return _type; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType) SimpleDbExpressionType.Attribute; }
        }

        public bool Equals(AttributeExpression x, AttributeExpression y)
        {
            if (x != null && y != null)
            {
                return x._name == y._name;
            }
            return x == y;
        }

        public int GetHashCode(AttributeExpression obj)
        {
            return obj._name.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("`{0}`", _name);
        }
    }
}