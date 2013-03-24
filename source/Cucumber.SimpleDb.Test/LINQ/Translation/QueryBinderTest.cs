using System;
using System.Linq;
using NUnit.Framework;
using Cucumber.SimpleDb.Utilities;
using Cucumber.SimpleDb.Linq.Translation;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;

namespace Cucumber.SimpleDb.Test
{
	[TestFixture]
	public class QueryBinderTest
	{
		[Test]
		public void WhereMethodIsTranslatable ()
		{
			var source = Enumerable.Empty<ISimpleDbItem>().AsQueryable();
			var binder = new QueryBinderAccessor();
			var lambdaParameter = Expression.Parameter(typeof(ISimpleDbItem));
			var lambda = Expression.Lambda(
				Expression.Constant(true),
				lambdaParameter);
			var whereMethod = Expression.Call(
				typeof(Queryable).GetMethod ("Where", typeof(IQueryable<Ref.T1>), typeof(Expression<Func<Ref.T1, bool>>))
					.MakeGenericMethod(typeof(ISimpleDbItem)),
				Expression.Constant(source),
				Expression.Quote(lambda));
			var resultExpression = binder.AccessVisitMethodCall(whereMethod);
			Assert.IsNotNull(resultExpression);
			Assert.IsInstanceOf<QueryExpression>(resultExpression);
			Assert.IsNull(((QueryExpression)resultExpression).Select);
			Assert.IsNull(((QueryExpression)resultExpression).Limit);
			Assert.IsEmpty(((QueryExpression)resultExpression).OrderBy);
			Assert.AreSame(lambda.Body, ((QueryExpression)resultExpression).Where);
		}

		private class QueryBinderAccessor : QueryBinder
		{
			public Expression AccessVisitMethodCall(MethodCallExpression m)
			{
				return this.VisitMethodCall(m);
			}
		}
	}
}

