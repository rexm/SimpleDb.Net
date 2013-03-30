using System;
using System.Linq;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture()]
    public class QueryOutputTest
    {
        [Test]
        public void WhereStringBasic ()
        {
            var query = GetQueryString(context => 
               context.Domains["TestDomain1"].Items.Where(i => i["TestAtt1"] == "TestValue1"));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"TestValue1\"", query);
        }

        [Test]
        public void WhereAndString()
        {
            var query = GetQueryString (context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                   i["TestAtt1"] == "hello" && i["TestAtt2"] == "world"));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"hello\" AND `TestAtt2` = \"world\"", query);
        }

        [Test]
        public void WhereContainsString()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                   i["TestAtt1"].StartsWith("searchFor")));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` LIKE \"searchFor%\"", query);
        }

        [Test]
        public void WhereNotStartsWithString()
        {
            var query = GetQueryString(context =>
               context.Domains["TestDomain1"].Items.Where(i =>
                   i["TestAtt1"].StartsWith("searchFor") == false));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` NOT LIKE \"searchFor%\"", query);
        }

        [Test]
        public void WhereNumberBasic()
        {
            var query = GetQueryString (context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                               i["TestAtt1"] == 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"1\"", query);
        }

        [Test]
        public void OrderByBasic()
        {
            var query = GetQueryString (context => 
                context.Domains["TestDomain1"].Items.OrderBy(i => i["TestAtt1"]));
            Assert.AreEqual("SELECT * FROM `TestDomain1` ORDERBY `TestAtt1` ASC", query);
        }

        [Test]
        public void OrderByComplex()
        {
            var query = GetQueryString (context => 
                context.Domains["TestDomain1"].Items
                   .OrderBy(i => i["TestAtt1"])
                    .OrderByDescending(i => i["TestAtt2"]));
            Assert.AreEqual("SELECT * FROM `TestDomain1` ORDERBY `TestAtt1` ASC, `TestAtt2` DESC", query);
        }

        [Test]
        public void BasicLimit()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                   .Where(i => i["TestAtt1"] > 0)
                   .Take(20));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` > \"0\" LIMIT 20", query);
        }

        private string GetQueryString(Func<ISimpleDbContext, IQueryable> query)
        {
            string output = null;
            var captureService = new QueryOutputCaptureService(val => output = val);
            using(var context = SimpleDbContext.Create(captureService))
            {
                foreach(var item in query(context))
                {
                    //no results
                }
            }
            return output;
        }
    }
}

