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
            if (m.Method.DeclaringType == typeof(Queryable))
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
                    case "First":
                        return BindFirst(m);
                    case "FirstOrDefault":
                        return BindFirstOrDefault(m);
                    case "OrderBy":
                    case "OrderByDescending":
                        return BindOrderBy(m);
                    default:
                        return BindClientEnumerable(m);
                }
            }
            if (m.Method.DeclaringType == typeof(SimpleDbQueryable))
            {
                switch (m.Method.Name)
                {
                    case "WithConsistency":
                        return BindUsingConsistency(m);
                }
            }
            return base.VisitMethodCall(m);
        }

        private Expression BindClientEnumerable(MethodCallExpression m)
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

        private Expression BindCount(MethodCallExpression m)
        {
            if (ShouldTransformMethod (m))
            {
                var source = m.Arguments[0];
                var elementType = TypeUtilities.GetElementType(m.Arguments[0].Type);
                if (m.Arguments.Count > 1)
                {
                    var predicate = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    source = Expression.Call(
                        new Func<IQueryable<object>,
                            Expression<Func<object, bool>>,
                            IQueryable<object>>(Queryable.Where<object>)
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType),
                        source,
                        predicate);
                }
                return BindCount(m.Type, source);
            }
            return BindClientEnumerable(m);
        }

        private Expression BindCount(Type type, Expression source)
        {
            source = this.Visit(source);
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
            var predicate = (LambdaExpression)StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindWhere(m.Type, m.Arguments[0], predicate);
            }
            return BindClientEnumerable(m);
        }

        private Expression BindWhere(Type type, Expression source, LambdaExpression predicate)
        {
            source = this.Visit(source);
            Expression where = Visit(predicate.Body);
            where = ItemAttributeMapper.Eval(where);
            return SimpleDbExpression.Query(null, source, where, null, null, false);
        }

        private Expression BindSelect(MethodCallExpression m)
        {
            var projector = (LambdaExpression)StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindSelect(m.Type, m.Arguments[0], projector);
            }
            return BindClientEnumerable(m);
        }

        private Expression BindSelect(Type type, Expression source, LambdaExpression projector)
        {
            source = this.Visit(source);
            Expression projectorBody = Visit(projector.Body);
            projectorBody = ItemAttributeMapper.Eval(projectorBody);
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
            var source = this.Visit(m.Arguments[0]);
            var limitExpression = this.Visit(m.Arguments[1]) as ConstantExpression;
            if(limitExpression == null)
            {
                throw new NotSupportedException("Cannot use an expression to determine the query LIMIT");
            }
            return SimpleDbExpression.Query(
                null, source, null, null, limitExpression, false);
        }

        private Expression BindFirst(MethodCallExpression m)
        {
            return BindFirst(m, Enumerable.First<object>);
        }

        private Expression BindFirstOrDefault(MethodCallExpression m)
        {
            return BindFirst(m, Enumerable.FirstOrDefault<object>);
        }

        private Expression BindFirst(
            MethodCallExpression m,
            Func<IEnumerable<object>, object> firstFunc)
        {
            if (ShouldTransformMethod(m))
            {
                var source = m.Arguments[0];
                var elementType = TypeUtilities.GetElementType(m.Arguments[0].Type);
                if (m.Arguments.Count > 1)
                {
                    var predicate = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    source = Expression.Call(
                        new Func<IQueryable<object>,
                            Expression<Func<object, bool>>,
                            IQueryable<object>>(Queryable.Where<object>)
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType),
                        source,
                        predicate);
                }
                source = Expression.Call(
                    new Func<IQueryable<object>,
                        int,
                        IQueryable<object>>(Queryable.Take)
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType),
                    source,
                    Expression.Constant(1));
                source = Expression.Call(
                    firstFunc.Method.GetGenericMethodDefinition().MakeGenericMethod(elementType),
                    source);
                return BindFirst(m.Type, source);
            }
            return BindClientEnumerable(m);
        }

        private Expression BindFirst(Type type, Expression source)
        {
            source = this.Visit(source);
            return SimpleDbExpression.Query(null, source, null, null, null, false);
        }

        private Expression BindJoin(MethodCallExpression m)
        {
            throw new NotImplementedException();
        }

        private Expression BindOrderBy(MethodCallExpression m)
        {
            SortDirection sortDirection = m.Method.Name.Contains("Descending") ? SortDirection.Descending : SortDirection.Ascending;
            var selector = (LambdaExpression)StripQuotes(m.Arguments[1]);
            if (ShouldTransformMethod(m))
            {
                return BindOrderBy(m.Type, m.Arguments[0], selector, sortDirection);
            }
            return BindClientEnumerable(m);
        }

        private Expression BindOrderBy(Type type, Expression source, LambdaExpression selector, SortDirection sortDirection)
        {
            source = this.Visit(source);
            Expression orderByBody = Visit(selector.Body);
            orderByBody = ItemAttributeMapper.Eval(orderByBody);
            var attributes = SelectionCollector.Collect(orderByBody);
            if(attributes.Count() < 1)
            {
                throw new InvalidOperationException("No attribute references found in the OrderBy expression");
            }
            if(attributes.Count() > 1)
            {
                throw new NotSupportedException("Currently only ordering by one column per order expression is supported");
            }
            return SimpleDbExpression.Query(
                null,
                source,
                null,
                new[] { new OrderExpression(attributes.First(), sortDirection) },
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
                if (mex.Arguments.Count > 1)
                {
                    return AdditionalArgumentIsLambdaCandidate(mex);
                }
                return true;
            }

            private static bool AdditionalArgumentIsLambdaCandidate(MethodCallExpression mex)
            {
                var lambda = StripQuotes (mex.Arguments[1]) as LambdaExpression;
                return lambda != null && typeof(ISimpleDbItem).IsAssignableFrom(lambda.Parameters[0].Type);
            }

            private static bool IsSupportedLinqMethod(MethodInfo m)
            {
                return (m.DeclaringType == typeof(Queryable) || m.DeclaringType == typeof(Enumerable))
                    && supportedLinqMethodNames.Contains(m.Name);
            }

            private static bool IsSupportedSimpleDbExtension(MethodInfo m)
            {
                return m.DeclaringType == typeof(SimpleDbQueryable);
            }

            private static readonly string[] supportedLinqMethodNames = { "Select", "Where", "Take", "OrderBy", "OrderByDescending", "Count", "First", "FirstOrDefault" };
        }
    }
}
