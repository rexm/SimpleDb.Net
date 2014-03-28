using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Cucumber.SimpleDb.Async.Linq.Structure;
using Cucumber.SimpleDb.Async.Utilities;

namespace Cucumber.SimpleDb.Async.Linq.Translation
{
    internal class QueryBinder : SimpleDbExpressionVisitor
    {
        public static Expression Eval(Expression exp)
        {
            return new QueryBinder().Visit(exp);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof (Queryable) || m.Method.DeclaringType == typeof (Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Count":
                        return BindCount(m);
                    case "Where":
                        return BindWhere(m);
                    case "Select":
                        return BindSelect(m);
                    case "Take":
                        return BindTake(m);
                    case "OrderBy":
                    case "OrderByDescending":
                    case "ThenByDescending":
                        return BindOrderBy(m);
                    default:
                        return BindDefaultEnumerable(m);
                }
            }
            if (m.Method.DeclaringType == typeof (SimpleDbQueryable))
            {
                switch (m.Method.Name)
                {
                    case "WithConsistency":
                        return BindUsingConsistency(m);
                }
            }
            return base.VisitMethodCall(m);
        }

        private Expression BindDefaultEnumerable(MethodCallExpression m)
        {
            var asEnumerableMethod = Expression.Call(
                typeof (Enumerable).GetMethod("AsEnumerable", BindingFlags.Static | BindingFlags.Public)
                    .MakeGenericMethod(
                        TypeUtilities.GetElementType(m.Arguments[0].Type)
                    ),
                Visit(m.Arguments[0]));
            var asQueryableMethod = Expression.Call(
                typeof (Queryable).GetMethods().First(mi => mi.Name == "AsQueryable" && mi.IsGenericMethod)
                    .MakeGenericMethod(
                        TypeUtilities.GetElementType(m.Arguments[0].Type)
                    ),
                asEnumerableMethod);
            return Expression.Call(
                m.Method,
                new[]
                {
                    asQueryableMethod
                }.Concat(
                    m.Arguments.Skip(1).Select(Visit)
                    ));
        }

        private Expression BindCount(MethodCallExpression m)
        {
            if (ShouldTransformMethod(m))
            {
                var source = m.Arguments[0];
                var elementType = TypeUtilities.GetElementType(m.Arguments[0].Type);
                if (m.Arguments.Count > 1)
                {
                    var predicate = (LambdaExpression) StripQuotes(m.Arguments[1]);
                    source = Expression.Call(
                        new Func<IQueryable<object>,
                            Expression<Func<object, bool>>,
                            IQueryable<object>>(Queryable.Where)
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType),
                        source,
                        predicate);
                }
                return BindCount(source);
            }
            return BindDefaultEnumerable(m);
        }

        private Expression BindCount(Expression source)
        {
            source = Visit(source);
            return SimpleDbExpression.Query(
                SimpleDbExpression.Count(),
                source,
                null,
                null,
                null,
                false);
        }

        private Expression BindWhere(MethodCallExpression m)
        {
            var predicate = (LambdaExpression) StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindWhere(m.Arguments[0], predicate);
            }
            return BindDefaultEnumerable(m);
        }

        private Expression BindWhere(Expression source, LambdaExpression predicate)
        {
            source = Visit(source);
            var where = Visit(predicate.Body);
            where = IndexedAttributeMapper.Eval(where);
            return SimpleDbExpression.Query(null, source, where, null, null, false);
        }

        private Expression BindSelect(MethodCallExpression m)
        {
            var projector = (LambdaExpression) StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindSelect(m.Arguments[0], projector);
            }
            return BindDefaultEnumerable(m);
        }

        private Expression BindSelect(Expression source, LambdaExpression projector)
        {
            source = Visit(source);
            var projectorBody = Visit(projector.Body);
            projectorBody = IndexedAttributeMapper.Eval(projectorBody);
            var attributes = SelectionCollector.Collect(projectorBody);
            return SimpleDbExpression.Project(
                SimpleDbExpression.Query(
                    new SelectExpression(attributes),
                    source,
                    null,
                    null,
                    null,
                    false),
                projector);
        }

        private Expression BindTake(MethodCallExpression m)
        {
            var source = Visit(m.Arguments[0]);
            var limitExpression = Visit(m.Arguments[1]) as ConstantExpression;
            if (limitExpression == null)
            {
                throw new NotSupportedException("Cannot use an expression to determine the query LIMIT");
            }
            return SimpleDbExpression.Query(
                null, source, null, null, limitExpression, false);
        }

        private Expression BindJoin(MethodCallExpression m)
        {
            throw new NotImplementedException();
        }

        private Expression BindOrderBy(MethodCallExpression m)
        {
            var sortDirection = m.Method.Name.Contains("Descending") ? SortDirection.Descending : SortDirection.Ascending;
            var selector = (LambdaExpression) StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindOrderBy(m.Arguments[0], selector, sortDirection);
            }
            return BindDefaultEnumerable(m);
        }

        private Expression BindOrderBy(Expression source, LambdaExpression selector, SortDirection sortDirection)
        {
            source = Visit(source);
            var orderByBody = Visit(selector.Body);
            orderByBody = IndexedAttributeMapper.Eval(orderByBody);
            var attributes = SelectionCollector.Collect(orderByBody);
            var attributeExpressions = attributes as IList<AttributeExpression> ?? attributes.ToList();
            if (!attributeExpressions.Any())
            {
                throw new InvalidOperationException("No attribute references found in the OrderBy expression");
            }
            if (attributeExpressions.Count() > 1)
            {
                throw new NotSupportedException("Currently only ordering by one column per order expression is supported");
            }
            return SimpleDbExpression.Query(
                null,
                source,
                null,
                new[]
                {
                    new OrderExpression(attributeExpressions.First(), sortDirection)
                },
                null,
                false);
        }

        private Expression BindUsingConsistency(MethodCallExpression m)
        {
            var source = Visit(m.Arguments[0]);
            return SimpleDbExpression.Query(
                null,
                source,
                null,
                null,
                null,
                true);
        }

        private ProjectionExpression VisitSequence(Expression expr)
        {
            return ConvertToSequence(base.Visit(expr));
        }

        private static ProjectionExpression ConvertToSequence(Expression expr)
        {
            switch (expr.NodeType)
            {
                case (ExpressionType) SimpleDbExpressionType.Projection:
                    return (ProjectionExpression) expr;
                case ExpressionType.New:
                    var nex = (NewExpression) expr;
                    if (expr.Type.IsGenericType && expr.Type.GetGenericTypeDefinition() == typeof (Grouping<,>))
                    {
                        return (ProjectionExpression) nex.Arguments[1];
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
                e = ((UnaryExpression) e).Operand;
            }
            return e;
        }

        private static bool ShouldTransformMethod(MethodCallExpression m)
        {
            return QueryNominator.IsCandidate(m);
        }

        private class QueryNominator : SimpleDbExpressionVisitor
        {
            private static readonly string[] SupportedLinqMethodNames =
            {
                "Select", "Where", "Take", "OrderBy", "OrderByDescending", "ThenByDescending", "Count"
            };

            private bool _isCandidate = true;

            public static bool IsCandidate(Expression source)
            {
                var nominator = new QueryNominator();
                nominator.Visit(source);
                return nominator._isCandidate;
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (MethodIsSupported(m.Method) && AdditionalArgumentsAreSupported(m))
                {
                    Visit(m.Arguments[0]);
                }
                else
                {
                    _isCandidate = false;
                }
                return m;
            }

            private static bool MethodIsSupported(MethodInfo m)
            {
                return IsSupportedLinqMethod(m) || IsSupportedSimpleDbExtension(m);
            }

            private static bool AdditionalArgumentsAreSupported(MethodCallExpression mex)
            {
                return mex.Arguments.Count <= 1 || AdditionalArgumentIsLambdaCandidate(mex);
            }

            private static bool AdditionalArgumentIsLambdaCandidate(MethodCallExpression mex)
            {
                var lambda = StripQuotes(mex.Arguments[1]) as LambdaExpression;
                return lambda != null && typeof (ISimpleDbItem).IsAssignableFrom(lambda.Parameters[0].Type);
            }

            private static bool IsSupportedLinqMethod(MethodInfo m)
            {
                return (m.DeclaringType == typeof (Queryable) || m.DeclaringType == typeof (Enumerable))
                       && SupportedLinqMethodNames.Contains(m.Name);
            }

            private static bool IsSupportedSimpleDbExtension(MethodInfo m)
            {
                return m.DeclaringType == typeof (SimpleDbQueryable);
            }
        }
    }
}