using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cucumber.SimpleDb.Async.Linq
{
    internal class SimpleDbQueryProvider : IQueryProvider
    {
        private readonly IInternalContext _context;

        internal SimpleDbQueryProvider(IInternalContext context)
        {
            _context = context;
        }

        public virtual object Execute(Expression expression)
        {
            var plan = CreateExecutionPlan(expression);
            plan = Expression.Convert(plan, typeof (object));
            return Expression.Lambda<Func<object>>(plan, null).Compile()();
        }

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            var elementType = TypeUtilities.GetElementType(expression.Type);
            try
            {
                return (IQueryable) Activator.CreateInstance(typeof (Query<>).MakeGenericType(elementType), new object[]
                {
                    this, expression
                });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return (TResult) Execute(expression);
        }

        protected virtual Expression CreateExecutionPlan(Expression expression)
        {
            try
            {
                expression = PartialEvaluator.Eval(expression, CanBeEvaluatedLocally);
                expression = QueryBinder.Eval(expression);
                expression = ImplicitSelect.EnsureQuery(expression);
                expression = DomainResolver.Resolve(expression);
                expression = QueryReducer.Reduce(expression);
                expression = SelectEnsurer.Ensure(expression);
                expression = QueryProxyWriter.Rewrite(expression, _context);
                return expression;
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    string.Format("Could not convert the query to valid SimpleDB syntax: {0}", ex.Message),
                    ex);
            }
        }

        private bool CanBeEvaluatedLocally(Expression expression)
        {
            var cex = expression as ConstantExpression;
            if (cex != null)
            {
                var query = cex.Value as IQueryable;
                if (query != null && query.Provider == this)
                {
                    return false;
                }
            }
            var mc = expression as MethodCallExpression;
            if (mc != null &&
                (mc.Method.DeclaringType == typeof (Enumerable) ||
                 mc.Method.DeclaringType == typeof (Queryable)))
            {
                return false;
            }
            if (expression.NodeType == ExpressionType.Convert &&
                expression.Type == typeof (object))
            {
                return true;
            }
            return expression.NodeType != ExpressionType.Parameter &&
                   expression.NodeType != ExpressionType.Lambda;
        }
    }
}