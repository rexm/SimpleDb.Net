using System.Linq;

namespace Cucumber.SimpleDb.Async.Linq.Structure
{
    internal abstract class ScalarExpression : SelectExpression
    {
        protected ScalarExpression()
            : base(Enumerable.Empty<AttributeExpression>())
        {
        }

        public abstract string ScalarAttributeName { get; }
    }
}