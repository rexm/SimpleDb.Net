using System;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Async.Linq.Structure
{
    internal class CountExpression : ScalarExpression
    {
        public override Type Type
        {
            get { return typeof (int); }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType) SimpleDbExpressionType.Count; }
        }

        public override string ScalarAttributeName
        {
            get { return "Count"; }
        }
    }
}