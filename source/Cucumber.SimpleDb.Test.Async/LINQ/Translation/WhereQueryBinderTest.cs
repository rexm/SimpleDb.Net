using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Async;
using Cucumber.SimpleDb.Async.Linq.Structure;
using Moq;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test.Async.LINQ.Translation
{
    [TestFixture]
    public class WhereQueryBinderTest : QueryBinderTest
    {
        [Test]
        public void WhereMethodIsTranslatable()
        {
            var source = new Mock<IQueryable<ISimpleDbItem>>().Object;
            var binder = new QueryBinderAccessor();
            var lambdaParameter = Expression.Parameter(typeof (ISimpleDbItem));
            var lambda = Expression.Lambda(
                Expression.Constant(true),
                lambdaParameter);
            var whereMethod = Expression.Call(
                new Func<IQueryable<object>, Expression<Func<object, bool>>, IQueryable<object>>(
                    Queryable.Where).Method
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(typeof (ISimpleDbItem)),
                Expression.Constant(source),
                Expression.Quote(lambda));
            var resultExpression = binder.AccessVisitMethodCall(whereMethod);
            Assert.IsNotNull(resultExpression);
            Assert.IsInstanceOf<QueryExpression>(resultExpression);
            Assert.IsNull(((QueryExpression) resultExpression).Select);
            Assert.IsNull(((QueryExpression) resultExpression).Limit);
            Assert.IsEmpty(((QueryExpression) resultExpression).OrderBy);
            Assert.AreSame(lambda.Body, ((QueryExpression) resultExpression).Where);
        }

        [Test]
        public void WhereMethodNotTranslatable()
        {
            var source = new Mock<IQueryable<string>>().Object;
            var binder = new QueryBinderAccessor();
            var lambda = Expression.Lambda(
                Expression.Constant(true),
                Expression.Parameter(typeof (string)));
            var whereMethod = Expression.Call(
                new Func<IQueryable<object>, Expression<Func<object, bool>>, IQueryable<object>>(
                    Queryable.Where).Method
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(typeof (string)),
                Expression.Constant(source),
                Expression.Quote(lambda));
            var resultExpression = binder.AccessVisitMethodCall(whereMethod);
            Assert.IsInstanceOf<MethodCallExpression>(resultExpression);
            Assert.AreEqual(whereMethod.Arguments[1], ((MethodCallExpression) resultExpression).Arguments[1]);
        }

        [Test]
        public void ClientMethodsGuardedByEnumerable()
        {
            var source = new Mock<IQueryable<string>>().Object;
            var binder = new QueryBinderAccessor();
            var lambda = Expression.Lambda(
                Expression.Constant(true),
                Expression.Parameter(typeof (string)));
            var whereMethod = Expression.Call(
                new Func<IQueryable<object>, Expression<Func<object, bool>>, IQueryable<object>>(
                    Queryable.Where).Method
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(typeof (string)),
                Expression.Constant(source),
                Expression.Quote(lambda));
            var resultExpression = binder.AccessVisitMethodCall(whereMethod);
            var resultAsMethodCall = resultExpression as MethodCallExpression;
            Assert.IsInstanceOf<MethodCallExpression>(resultAsMethodCall.Arguments[0]);
            Assert.AreSame(
                new Func<IEnumerable<object>, IQueryable<object>>(Queryable.AsQueryable)
                    .Method.GetGenericMethodDefinition()
                    .MakeGenericMethod(typeof (string)),
                ((MethodCallExpression) resultAsMethodCall.Arguments[0]).Method);
            Assert.IsInstanceOf<MethodCallExpression>(
                ((MethodCallExpression) resultAsMethodCall.Arguments[0]).Arguments[0]);
            Assert.AreSame(
                new Func<IEnumerable<object>, IEnumerable<object>>(Enumerable.AsEnumerable)
                    .Method.GetGenericMethodDefinition()
                    .MakeGenericMethod(typeof (string)),
                ((MethodCallExpression) ((MethodCallExpression) resultAsMethodCall.Arguments[0]).Arguments[0]).Method);
        }
    }
}