using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cucumber.SimpleDb.Async;
using Cucumber.SimpleDb.Async.Session;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test.Async.LINQ.IntegrationTests
{
    [TestFixture]
    public class QueryOutputTest
    {
        [Test]
        public async Task ImplicitSelect()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items);
            Assert.AreEqual("SELECT * FROM `TestDomain1`", query);
        }

        [Test]
        public async Task WhereStringBasic()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items.Where(i => i["TestAtt1"] == "TestValue1"));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"TestValue1\"", query);
        }

        [Test]
        public async Task WhereAndString()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items.Where(i =>
                    i["TestAtt1"] == "hello" && i["TestAtt2"] == "world"));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"hello\" AND `TestAtt2` = \"world\"", query);
        }

        [Test]
        public async Task WhereContainsString()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items.Where(i =>
                    i["TestAtt1"].StartsWith("searchFor")));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` LIKE \"searchFor%\"", query);
        }

        [Test]
        public async Task WhereNotStartsWithString()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items.Where(i =>
                    i["TestAtt1"].StartsWith("searchFor") == false));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` NOT LIKE \"searchFor%\"", query);
        }

        [Test]
        public async Task WhereNumberBasic()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items.Where(i =>
                    i["TestAtt1"] == 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"1\"", query);
        }

        [Test]
        public async Task OrderByBasic()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items.OrderBy(i => i["TestAtt1"]));
            Assert.AreEqual("SELECT * FROM `TestDomain1` ORDERBY `TestAtt1` ASC", query);
        }

        [Test]
        public async Task OrderByComplex()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items
                    .OrderBy(i => i["TestAtt1"])
                    .OrderByDescending(i => i["TestAtt2"]));
            Assert.AreEqual("SELECT * FROM `TestDomain1` ORDERBY `TestAtt1` ASC, `TestAtt2` DESC", query);
        }

        [Test]
        public async Task LimitBasic()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items
                    .Where(i => i["TestAtt1"] > 0)
                    .Take(20));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` > \"0\" LIMIT 20", query);
        }

        [Test]
        public async Task InBasic()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items
                    .Where(i => i["TestAtt1"].In(1, 2, 3)));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` IN( \"1\", \"2\", \"3\" )", query);
        }

        [Test]
        public async Task BetweenBasic()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items
                    .Where(i => i["TestAtt1"].Between(7.5, 50)));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` BETWEEN \"7.5\" AND \"50\"", query);
        }

        [Test]
        public async Task Every()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items
                    .Where(i => i["TestAtt1"].Every() > 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE every( `TestAtt1` ) > \"1\"", query);
        }

        [Test]
        public async Task CountBasic()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items
                    .CountAsync());
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1`", query);
        }

        [Test]
        public async Task CountWhere()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items
                    .Where(i => i["TestAtt1"] > 1)
                    .CountAsync());
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1` WHERE `TestAtt1` > \"1\"", query);
        }

        [Test]
        public async Task CountWithPredicate()
        {
            var query = await GetQueryString(async context =>
                (await context.GetDomainAsync("TestDomain1")).Items
                    .CountAsync(i => i["TestAtt1"] > 1));
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1` WHERE `TestAtt1` > \"1\"", query);
        }

        private static async Task<string> GetQueryString<T>(Func<ISimpleDbContext, Task<T>> query)
        {
            string output = null;
            var captureService = new QueryOutputCaptureService(val => output = val);
            using (var context = new SimpleDbContextWithTestDomain(captureService, new SimpleDbSession(captureService)))
            {
                if (typeof (IQueryable).IsAssignableFrom(typeof (T)))
                {
                    await (await query(context) as IQueryable).ForEachAsync(o => { });
                }
                else
                {
                    await query(context);
                }
            }
            return output;
        }
    }
}