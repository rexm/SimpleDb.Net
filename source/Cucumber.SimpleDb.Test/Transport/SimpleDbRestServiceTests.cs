using NUnit.Framework;
using System;
using System.Linq;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;

using System.Collections.Generic;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture ()]
    public class SimpleDbRestServiceTests
    {
        [Test ()]
        public void GenerateBatchPutAttributes ()
        {
            var service = new SimpleDbRestService (new PassThroughAwsRestService ());
            var result = service.BatchPutAttributes ("TestDomain1",
                new { Name = "TestItem1", Attributes = new object[] {
                        new { Name = "TestAtt1", Value = "Hello" },
                        new { Name = "TestAtt2", Value = "World" },
                        new { Name = "TestAtt3", Value = new { Value="abc,123", Values = new List<object> {"abc", 123}} }
                    }
                    },
                new { Name = "TestItem2", Attributes = new object[] {
                        new { Name = "TestAtt4", Value = 123 },
                        new { Name = "TestAtt5", Value = 1.23 },
                        new { Name = "TestAtt6", Value = new { Value="abc,123", Values = new List<object> {"abc", 123}} }
                    }
                });
            Assert.AreEqual (result.Elements ().Count (), 20);
            //TODO: more comprehensive check
        }
    }
}

