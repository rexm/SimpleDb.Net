using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Cucumber.SimpleDb.Async.Linq.Translation;
using Cucumber.SimpleDb.Async.Utilities;

namespace Cucumber.SimpleDb.Async.Linq
{
    internal sealed class SimpleDbQueryProvider : IAsyncQueryProvider
    {
        private readonly IInternalContext _context;

        internal SimpleDbQueryProvider(IInternalContext context)
        {
            _context = context;
        }

        public object Execute(Expression expression)
        {
            var plan = CreateExecutionPlan(expression);
            plan = Expression.Convert(plan, typeof (object));
            return Expression.Lambda<Func<object>>(plan, null).Compile()();
        }

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new SimpleDbQuery<TElement>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            var elementType = TypeUtilities.GetElementType(expression.Type);
            try
            {
                return (IQueryable) Activator.CreateInstance(typeof (SimpleDbQuery<>).MakeGenericType(elementType), new object[]
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

        Task<TResult> IAsyncQueryProvider.ExecuteScalarAsync<TResult>(Expression expression)
        {
            var plan = CreateExecutionPlan(expression);

            plan = Expression.Convert(plan, typeof(object));

            return (Task<TResult>)Expression.Lambda<Func<object>>(plan, null).Compile()();
        }

        Task<IEnumerable<TResult>> IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression)
        {
            var plan = CreateExecutionPlan(expression);

            plan = Expression.Convert(plan, typeof(object));

            return (Task<IEnumerable<TResult>>)Expression.Lambda<Func<object>>(plan, null).Compile()();
        }

        internal Expression CreateExecutionPlan(Expression expression)
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