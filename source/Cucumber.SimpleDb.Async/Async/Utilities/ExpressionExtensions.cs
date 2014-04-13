using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Async.Utilities
{
    internal static class ExpressionExtensions
    {
        public static Expression ReduceTotally(this Expression expression)
        {
            while (expression.CanReduce)
            {
                expression = expression.Reduce();
            }
            return expression;
        }
    }
}