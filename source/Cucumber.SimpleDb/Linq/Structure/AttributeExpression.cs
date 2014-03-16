using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

namespace Cucumber.SimpleDb.Linq.Structure
{
    [DebuggerDisplay("`{Name}`")]
    internal class AttributeExpression : SimpleDbExpression, IEqualityComparer<AttributeExpression>
    {
        private string _name;
        private Type _type;

        public AttributeExpression(string name, Type type)
            : base()
        {
            _name = name;
            _type = type;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public override Type Type
        {
            get
            {
                return _type;
            }
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)SimpleDbExpressionType.Attribute;
            }
        }

        public override string ToString()
        {
            return string.Format("`{0}`", _name);
        }

        public bool Equals(AttributeExpression x, AttributeExpression y)
        {
            if (x != null && y != null)
            {
                return x._name == y._name;
            }
            else
            {
                return (object)x == (object)y;
            }
        }

        public int GetHashCode(AttributeExpression obj)
        {
            return obj._name.GetHashCode();
        }
    }
}
