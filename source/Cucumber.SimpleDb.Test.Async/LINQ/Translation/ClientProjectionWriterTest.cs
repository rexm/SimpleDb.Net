using System.Linq.Expressions;
using Cucumber.SimpleDb.Async;
using Cucumber.SimpleDb.Async.Linq.Structure;
using Cucumber.SimpleDb.Async.Linq.Translation;
using Moq;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test.Async.Linq.Translation
{
    [TestFixture]
    public class ClientProjectionWriterTest
    {
        [Test]
        public void EnsureSourceExpressionIsUntouched()
        {
            var sourceExpression = SimpleDbExpression.Query(null, null, null, null, null, false);
            var projectionExpression = SimpleDbExpression.Project(sourceExpression, null);
            var resultExpression = ClientProjectionWriter.Rewrite(projectionExpression);
            Assert.IsNotNull(resultExpression);
            Assert.AreSame(sourceExpression, resultExpression.Source);
        }

        [Test]
        public void CreateDefaultProjector()
        {
            var projectionExpression = SimpleDbExpression.Project(null, null);
            var resultExpression = ClientProjectionWriter.Rewrite(projectionExpression);
            Assert.IsNotNull(resultExpression);
            Assert.IsInstanceOf<LambdaExpression>(resultExpression.Projector);
            Assert.AreEqual(1, ((LambdaExpression) resultExpression.Projector).Parameters.Count);
            Assert.AreEqual(typeof (ISimpleDbItem), ((LambdaExpression) resultExpression.Projector).Parameters[0].Type);
            var projectorFunction = ((LambdaExpression) resultExpression.Projector).Compile();
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