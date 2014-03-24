using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Translation;

namespace Cucumber.SimpleDb.Test
{
    public abstract class QueryBinderTest
    {
        internal class QueryBinderAccessor : QueryBinder
        {
            public Expression AccessVisitMethodCall(MethodCallExpression m)
            {
                return VisitMethodCall(m);
            }
        }
    }
}