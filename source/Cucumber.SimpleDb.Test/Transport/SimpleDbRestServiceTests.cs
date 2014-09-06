using NUnit.Framework;
using System;
using System.Linq;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;
using System.Collections.Generic;
using Cucumber.SimpleDb.Test.Transport;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class SimpleDbRestServiceTests
    {
        [Test]
        public async void GenerateBatchPutAttributes()
        {
            var service = new SimpleDbRestService(new PassThroughAwsRestService());
            var result = await service.BatchPutAttributesAsync("TestDomain1",
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
            }).ConfigureAwait(false);
            Assert.AreEqual(result.Elements().Count(), 20);
            //TODO: more comprehensive check
        }

        [Test]
        public async void TestCreateValidDomainName()
        {
            var domainName = "A_Domain_With_Valid_Chars-1.0";
            var service = new SimpleDbRestService(new PassThroughAwsRestService ());
            var result = await service.CreateDomainAsync(domainName).ConfigureAwait(false);
            Assert.IsTrue (result.Elements ("Argument")
                .FirstOrDefault (x =>
                    x.Element ("Key").Value == "DomainName" &&
                    x.Element ("Value").Value == domainName) != null);
        }

        [Test]
        public void TestCreateInvalidDomainNameTooShort()
        {
            var domainName = "ab";
            var service = new SimpleDbRestService(new PassThroughAwsRestService ());
            var exception = Assert.Catch(async () =>
            {
                await service.CreateDomainAsync(domainName).ConfigureAwait(false);
            });
            Assert.IsInstanceOf<FormatException>(exception);
        }

        [Test]
        public void TestCreateInvalidDomainNameTooLong()
        {
            var domainName = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz";
            var service = new SimpleDbRestService(new PassThroughAwsRestService ());
            var exception = Assert.Catch(async () =>
                {
                    await service.CreateDomainAsync(domainName).ConfigureAwait(false);
                });
            Assert.IsInstanceOf<FormatException>(exception);
        }

        [Test]
        public void TestCreateInvalidDomainNameBadChars()
        {
            var domainName = "Domain_$_Name";
            var service = new SimpleDbRestService(new PassThroughAwsRestService ());
            var exception = Assert.Catch(async () =>
                {
                    await service.CreateDomainAsync(domainName).ConfigureAwait(false);
                });
            Assert.IsInstanceOf<FormatException>(exception);
        }
    }
}

