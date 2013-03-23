using System;
using System.Reflection;
using NUnit.Framework;
using Cucumber.SimpleDb.Linq.Structure;
using Cucumber.SimpleDb.Linq.Translation;
using System.Linq.Expressions;
using Moq;

namespace Cucumber.SimpleDb.Test
{
	[TestFixture]
	public class ClientProjectionWriterTest
	{
		[Test]
		public void EnsureSourceExpressionIsUntouched()
		{
			var sourceExpression = new QueryExpression(null, null, null, null);
			var projectionExpression = new ProjectionExpression(sourceExpression, null);
			var resultExpression = ClientProjectionWriter.Rewrite(projectionExpression);
			Assert.IsNotNull(resultExpression);
			Assert.AreSame(sourceExpression, resultExpression.Source);
		}

		[Test]
		public void CreateDefaultProjector ()
		{
			var projectionExpression = new ProjectionExpression(null, null);
			var resultExpression = ClientProjectionWriter.Rewrite(projectionExpression);
			Assert.IsNotNull(resultExpression);
			Assert.IsInstanceOf<LambdaExpression>(resultExpression.Projector);
			Assert.AreEqual(1, ((LambdaExpression)resultExpression.Projector).Parameters.Count);
			Assert.AreEqual(typeof(ISimpleDbItem), ((LambdaExpression)resultExpression.Projector).Parameters[0].Type);
			var projectorFunction = ((LambdaExpression)resultExpression.Projector).Compile();
			var inputItem = new Mock<ISimpleDbItem>().Object;
			var projectionResult = projectorFunction.DynamicInvoke(inputItem);
			Assert.AreSame(inputItem, projectionResult);
		}

		[Test]
		public void CreateSimpleResultClientProjector()
		{
		}

		[Test]
		public void CreateTypedResultClientProjector()
		{
		}
	}
}

