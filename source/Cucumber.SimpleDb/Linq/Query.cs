// Based on the Query<T> class by Matt Warren:
// http://iqtoolkit.codeplex.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace Cucumber.SimpleDb.Linq
{
    internal class Query<T> : IOrderedQueryable<T>
    {
        private IQueryProvider _provider;
        private Expression _expression;

        public Query(IQueryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            this._provider = provider;
            this._expression = Expression.Constant(this);
        }

        public Query(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            this._provider = provider;
            this._expression = expression;
        }

        Expression IQueryable.Expression
        {
            get { return this._expression; }
        }

        Type IQueryable.ElementType
        {
            get { return typeof(T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return this._provider; }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this._provider.Execute(this._expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this._provider.Execute(this._expression)).GetEnumerator();
        }

        public override string ToString()
        {
            if (this._expression.NodeType == ExpressionType.Constant &&
                ((ConstantExpression)this._expression).Value == this)
            {
                return "Query(" + typeof(T) + ")";
            }
            else
            {
                return this._expression.ToString();
            }
        }
    }
}
