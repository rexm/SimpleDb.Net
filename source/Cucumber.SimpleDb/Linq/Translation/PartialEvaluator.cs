// Based on the PartialEvaluator class by Matt Warren:
// http://iqtoolkit.codeplex.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal static class PartialEvaluator
    {
        public static Expression Eval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return SubtreeEvaluator.Eval(Nominator.Nominate(fnCanBeEvaluated, expression), expression);
        }

        public static Expression Eval(Expression expression)
        {
            return Eval(expression, PartialEvaluator.CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        private class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly HashSet<Expression> _candidates;

            private SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this._candidates = candidates;
            }

            internal static Expression Eval(HashSet<Expression> candidates, Expression exp)
            {
                return new SubtreeEvaluator(candidates).Visit(exp);
            }

            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (this._candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }
                return base.Visit(exp);
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                Type type = e.Type;
                if (type.IsValueType)
                {
                    e = Expression.Convert(e, typeof(object));
                }
                Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(e);
                Func<object> fn = lambda.Compile();
                return Expression.Constant(fn(), type);
            }
        }

        private class Nominator : ExpressionVisitor
        {
            private readonly Func<Expression, bool> _fnCanBeEvaluated;
            private readonly HashSet<Expression> _candidates;
            private bool _cannotBeEvaluated;

            private Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this._candidates = new HashSet<Expression>();
                this._fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal static HashSet<Expression> Nominate(Func<Expression, bool> fnCanBeEvaluated, Expression expression)
            {
                Nominator nominator = new Nominator(fnCanBeEvaluated);
                nominator.Visit(expression);
                return nominator._candidates;
            }

            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this._cannotBeEvaluated;
                    this._cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!this._cannotBeEvaluated)
                    {
                        if (this._fnCanBeEvaluated(expression))
                        {
                            this._candidates.Add(expression);
                        }
                        else
                        {
                            this._cannotBeEvaluated = true;
                        }
                    }
                    this._cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
        }
    }
}
