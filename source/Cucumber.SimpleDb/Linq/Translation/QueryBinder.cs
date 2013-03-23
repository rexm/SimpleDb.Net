using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;
using System.Reflection;
using Cucumber.SimpleDb.Utilities;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class QueryBinder : SimpleDbExpressionVisitor
    {
        public static Expression Eval(Expression exp)
        {
            return new QueryBinder().Visit(exp);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) || m.Method.DeclaringType == typeof(Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Where":
                        return BindWhere(m);
                    case "Select":
                        return BindSelect(m);
                    case "OrderBy":
                    case "OrderByDescending":
                        return BindOrderBy(m);
                    default:
                        return BindDefaultEnumerable(m);
                }
            }
            return base.VisitMethodCall(m);
        }

        private Expression BindDefaultEnumerable(MethodCallExpression m)
        {
            var asEnumerableMethod = Expression.Call(
                typeof(Enumerable).GetMethod("AsEnumerable", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(
                    TypeUtilities.GetElementType(m.Arguments[0].Type)
                ),
                this.Visit(m.Arguments[0]));
            var asQueryableMethod = Expression.Call(
                typeof(Queryable).GetMethods().First(mi => mi.Name == "AsQueryable" && mi.IsGenericMethod)
                .MakeGenericMethod(
                    TypeUtilities.GetElementType(m.Arguments[0].Type)
                ),
                asEnumerableMethod);
            return Expression.Call(
                m.Method,
                new[] { asQueryableMethod }.Concat(
                    m.Arguments.Skip(1).Select(arg =>
                        this.Visit(arg)
                    )
                ));
        }

        private Expression BindWhere(MethodCallExpression m)
        {
            var predicate = (LambdaExpression)StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindWhere(m.Type, m.Arguments[0], predicate);
            }
            return BindDefaultEnumerable(m);
        }

        private Expression BindWhere(Type type, Expression source, LambdaExpression predicate)
        {
            source = this.Visit(source);
            Expression where = Visit(predicate.Body);
            where = IndexedAttributeMapper.Eval(where);
            return new QueryExpression(null, source, where, null);
        }

        private Expression BindSelect(MethodCallExpression m)
        {
            var projector = (LambdaExpression)StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindSelect(m.Type, m.Arguments[0], projector);
            }
            return BindDefaultEnumerable(m);
        }

        private Expression BindSelect(Type type, Expression source, LambdaExpression projector)
        {
            source = this.Visit(source);
            Expression projectorBody = Visit(projector.Body);
            projectorBody = IndexedAttributeMapper.Eval(projectorBody);
            var attributes = SelectionCollector.Collect(projectorBody);
            return new ProjectionExpression(
                new QueryExpression(
                    new SelectExpression(attributes),
                    source,
                    null,
                    null),
                projector);
        }

		private Expression BindJoin (MethodCallExpression m)
		{
			throw new NotImplementedException ();
		}

        private Expression BindOrderBy(MethodCallExpression m)
        {
            SortDirection sortDirection = m.Method.Name.Contains("Descending") ? SortDirection.Descending : SortDirection.Ascending;
            var selector = (LambdaExpression)StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindOrderBy(m.Type, m.Arguments[0], selector, sortDirection);
            }
            return BindDefaultEnumerable(m);
        }

        private Expression BindOrderBy(Type type, Expression source, LambdaExpression selector, SortDirection sortDirection)
        {
            source = this.Visit(source);
            Expression orderByBody = Visit(selector.Body);
            orderByBody = IndexedAttributeMapper.Eval(orderByBody);
            var attributes = SelectionCollector.Collect(orderByBody);
			if(attributes.Count () < 1)
			{
				throw new InvalidOperationException("No attribute references found in the OrderBy expression");
			}
			if(attributes.Count () > 1)
			{
				throw new NotSupportedException("Currently only ordering by one column per order expression is supported");
			}
            return new QueryExpression(
                null,
                source,
                null,
                new[] { new OrderExpression(attributes.First(), sortDirection) });
        }

        private ProjectionExpression VisitSequence(Expression expr)
        {
            return ConvertToSequence(base.Visit(expr));
        }

        private ProjectionExpression ConvertToSequence(Expression expr)
        {
            switch (expr.NodeType)
            {
                case (ExpressionType)SimpleDbExpressionType.Projection:
                    return (ProjectionExpression)expr;
                case ExpressionType.New:
                    NewExpression nex = (NewExpression)expr;
                    if (expr.Type.IsGenericType && expr.Type.GetGenericTypeDefinition() == typeof(Grouping<,>))
                    {
                        return (ProjectionExpression)nex.Arguments[1];
                    }
                    goto default;
                default:
                    throw new Exception(string.Format("The expression of type '{0}' is not a sequence", expr.Type));
            }
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        private bool ShouldTransformMethod(MethodCallExpression m)
        {
            return QueryNominator.IsCandidate(m);
        }

        private class QueryNominator : SimpleDbExpressionVisitor
        {
            public static bool IsCandidate(Expression source)
            {
                var nominator = new QueryNominator();
                nominator.Visit(source);
                return nominator._isCandidate;
            }

            private bool _isCandidate = true;

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                var argument = StripQuotes(m.Arguments[1]);
                var lambda = argument as LambdaExpression;
                if (lambda != null && MethodIsSupported(m.Method) && typeof(ISimpleDbItem).IsAssignableFrom(lambda.Parameters[0].Type))
                {
                    Visit(m.Arguments[0]);
                }
                else
                {
                    _isCandidate = false;
                }
                return m;
            }

            private bool MethodIsSupported(MethodInfo m)
            {
                return (m.DeclaringType == typeof(Queryable) || m.DeclaringType == typeof(Enumerable))
                    && supportedMethodNames.Contains(m.Name);
            }

            private static readonly string[] supportedMethodNames = { "Select", "Where", "OrderBy", "OrderByDescending" };
        }
    }
}
