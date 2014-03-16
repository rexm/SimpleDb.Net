using System;
using System.Linq;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal abstract class ScalarExpression : SelectExpression
    {
        public ScalarExpression()
            : base(Enumerable.Empty<AttributeExpression>())
        {
        }

        public abstract string ScalarAttributeName
        {
            get;
        }
    }
}

