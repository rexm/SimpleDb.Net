using System;
using System.Linq;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;
using Cucumber.SimpleDb.Linq.Translation;
using NUnit.Framework;
using Moq;


namespace Cucumber.SimpleDb.Test
{
    [TestFixture()]
    public class IndexedAttributeMapperTest
    {
        [Test()]
        public void ConvertBasicAccessorInsidePredicate ()
        {
            var item = new Mock<ISimpleDbItem> ();
            var methodCallExp = Expression.Call(
                Expression.Constant(item.Object),
                typeof(ISimpleDbItem).GetMethod ("get_Item"),
                Expression.Constant ("TestAttribute"));

            var mapper = new IndexedAttributeMapperAccessor ();
            var mappedAttribute = mapper.AccessVisitMethodCall (methodCallExp);

            Assert.IsNotNull (mappedAttribute);
            Assert.IsInstanceOf<AttributeExpression> (mappedAttribute);
            Assert.AreEqual (((AttributeExpression)mappedAttribute).Name, "TestAttribute");
        }

        private class IndexedAttributeMapperAccessor : IndexedAttributeMapper
        {
            public Expression AccessVisitMethodCall(MethodCallExpression m)
            {
                return this.VisitMethodCall (m);
            }

            public Expression AccessVisitUnary(UnaryExpression u)
            {
                return this.VisitUnary (u);
            }

            public Expression AccessVisitNew(NewExpression n)
            {
                return this.VisitNew(n);
            }
        }
    }
}

