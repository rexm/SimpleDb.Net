using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;
using System.ComponentModel;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal class CountExpression : ScalarExpression
	{
        public override Type Type
        {
            get
            {
                return typeof(int);
            }
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)SimpleDbExpressionType.Count;
            }
        }

        public override string ScalarAttributeName
        {
            get
            {
                return "Count";
            }
        }
	}
}
