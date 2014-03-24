using System.Linq;
using Cucumber.SimpleDb.Transport;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class SimpleDbRestServiceTests
    {
        [Test]
        public void GenerateBatchPutAttributes()
        {
            var service = new SimpleDbRestService(new PassThroughAwsRestService());
            var result = service.BatchPutAttributes("TestDomain1",
                new
                {
                    Name = "TestItem1",
                    Attributes = new object[]
                    {
                        new
                        {
                            Name = "TestAtt1",
                            Value = "Hello"
                        },
                        new
                        {
                            Name = "TestAtt2",
                            Value = "World"
                        }
                    }
                },
                new
                {
                    Name = "TestItem2",
                    Attributes = new object[]
                    {
                        new
                        {
                            Name = "TestAtt3",
                            Value = 123
                        },
                        new
                        {
                            Name = "TestAtt4",
                            Value = 1.23
                        }
                    }
                });
            Assert.AreEqual(result.Elements().Count(), 12);
            //TODO: more comprehensive check
        }
    }
}