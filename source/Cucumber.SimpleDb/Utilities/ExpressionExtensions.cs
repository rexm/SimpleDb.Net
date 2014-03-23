using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Utilities
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