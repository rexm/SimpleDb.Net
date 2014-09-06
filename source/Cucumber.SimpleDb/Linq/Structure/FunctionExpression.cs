using System;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Collections.Generic;

namespace Cucumber.SimpleDb.Linq.Structure
{
    [DebuggerDisplay("{name}()")]
    internal class FunctionExpression : AttributeExpression
    {
        public FunctionExpression(string functionName, Type type)
            : base(functionName, type)
        {
        }

        public override string ToString()
        {
            return string.Format("{0}()", this.Name);
        }
    }
}

