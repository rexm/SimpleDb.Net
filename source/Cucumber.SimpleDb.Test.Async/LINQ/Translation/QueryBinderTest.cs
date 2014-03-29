using System.Linq.Expressions;
using Cucumber.SimpleDb.Async.Linq.Translation;

namespace Cucumber.SimpleDb.Test.Async.Linq.Translation
{
    public abstract class QueryBinderTest
    {
        internal sealed class QueryBinderAccessor : QueryBinder
        {
            public Expression AccessVisitMethodCall(MethodCallExpression m)
            {
                return VisitMethodCall(m);
            }
        }
    }
}