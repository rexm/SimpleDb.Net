using System;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Async;
using Cucumber.SimpleDb.Async.Linq.Structure;
using Cucumber.SimpleDb.Async.Linq.Translation;
using Moq;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test.Async.Linq.Translation
{
    [TestFixture]
    public class IndexedAttributeMapperTest
    {
        [Test]
        public void ConvertBasicAccessorExpression()
        {
            var item = new Mock<ISimpleDbItem>();
            var methodCallExp = Expression.Call(
                Expression.Constant(item.Object),
                typeof (ISimpleDbItem).GetMethod("get_Item"),
                Expression.Constant("TestAttribute"));

            var mapper = new IndexedAttributeMapperAccessor();
            var mappedAttribute = mapper.AccessVisitMethodCall(methodCallExp);

            Assert.IsNotNull(mappedAttribute);
            Assert.IsInstanceOf<AttributeExpression>(mappedAttribute);
            Assert.AreEqual("TestAttribute", ((AttributeExpression) mappedAttribute).Name);
        }

        [Test]
        public void SkipNonAccessorExpression()
        {
            var item = new Mock<ISimpleDbItem>();
            var methodCallExp = Expression.Call(
                Expression.Constant(item.Object),
                typeof (object).GetMethod("ToString"));

            var mapper = new IndexedAttributeMapperAccessor();
            var returnExpression = mapper.AccessVisitMethodCall(methodCallExp);
            Assert.IsNotNull(returnExpression);
            Assert.IsInstanceOf<MethodCallExpression>(returnExpression);
            Assert.IsInstanceOf<ConstantExpression>(((MethodCallExpression) returnExpression).Object);
            Assert.AreEqual(item.Object, ((ConstantExpression) ((MethodCallExpression) returnExpression).Object).Value);
            Assert.AreEqual(
                typeof (object).GetMethod("ToString"),
                ((MethodCallExpression) returnExpression).Method);
        }

        [Test]
        public void ConstructorWithAccessorOnly()
        {
            var item = new Mock<ISimpleDbItem>().Object;
            Expression<Func<object>> lambdaWithNew = () => new
            {
                Test1 = item["TestAttribute"]
            };
            var newExp = lambdaWithNew.Body as NewExpression;
            var mapper = new IndexedAttributeMapperAccessor();
            var returnExpression = mapper.AccessVisitNew(newExp);
            Assert.IsNotNull(returnExpression);
            Assert.IsInstanceOf<NewExpression>(returnExpression);
            Assert.AreEqual(1, ((NewExpression) returnExpression).Arguments.Count);
            Assert.IsInstanceOf<AttributeExpression>(((NewExpression) returnExpression).Arguments[0]);
            Assert.AreEqual("TestAttribute", ((AttributeExpression) ((NewExpression) returnExpression).Arguments[0]).Name);
        }

        private class IndexedAttributeMapperAccessor : IndexedAttributeMapper
        {
            public Expression AccessVisitMethodCall(MethodCallExpression m)
            {
                return VisitMethodCall(m);
            }

            public Expression AccessVisitUnary(UnaryExpression u)
            {
                return VisitUnary(u);
            }

            public Expression AccessVisitNew(NewExpression n)
            {
                return VisitNew(n);
            }
        }
    }
}