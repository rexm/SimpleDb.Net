// Based on the Query<T> class by Matt Warren:
// http://iqtoolkit.codeplex.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cucumber.SimpleDb.Async.Infrastructure;

namespace Cucumber.SimpleDb.Async.Linq
{
    internal class SimpleDbQuery<T> : IOrderedQueryable<T>, IEnumerableAsync<T>
    {
        private readonly Expression _expression;
        private readonly IAsyncQueryProvider _provider;

        public SimpleDbQuery(IAsyncQueryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public SimpleDbQuery(IAsyncQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof (IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            _provider = provider;
            _expression = expression;
        }

        Expression IQueryable.Expression
        {
            get { return _expression; }
        }

        Type IQueryable.ElementType
        {
            get { return typeof (T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _provider; }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _provider.Execute(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _provider.Execute(_expression)).GetEnumerator();
        }

        async Task<IEnumerable<T>> IEnumerableAsync<T>.GetEnumerableAsync()
        {
            return await _provider.ExecuteAsync<T>(_expression).ConfigureAwait(false);
        }

        public override string ToString()
        {
            if (_expression.NodeType == ExpressionType.Constant &&
                ((ConstantExpression) _expression).Value == this)
            {
                return "Query(" + typeof (T) + ")";
            }
            return _expression.ToString();
        }
    }
}