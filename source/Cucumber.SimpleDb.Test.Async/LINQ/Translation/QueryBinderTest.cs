using System.Linq.Expressions;
using Cucumber.SimpleDb.Async.Linq.Translation;

namespace Cucumber.SimpleDb.Test.Async.LINQ.Translation
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