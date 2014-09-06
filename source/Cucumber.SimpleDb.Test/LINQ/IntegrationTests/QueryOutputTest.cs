using System;
using System.Linq;
using NUnit.Framework;
using Cucumber.SimpleDb.Test.Transport;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class QueryOutputTest
    {
        [Test]
        public async void ImplicitSelect()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items);
            Assert.AreEqual("SELECT * FROM `TestDomain1`", query);
        }

        [Test]
        public async void WhereStringBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i => i["TestAtt1"] == "TestValue1"));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"TestValue1\"", query);
        }

        [Test]
        public async void WhereAndString()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                    i["TestAtt1"] == "hello" && i["TestAtt2"] == "world"));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"hello\" AND `TestAtt2` = \"world\"", query);
        }

        [Test]
        public async void WhereContainsString()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                   i["TestAtt1"].Contains("searchFor")));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` LIKE \"%searchFor%\"", query);
        }

		[Test]
		public async void WhereNotContainsString()
		{
			var query = await GetQueryString(context =>
				context.Domains["TestDomain1"].Items.Where(i =>
					!i["TestAtt1"].Contains("searchFor")));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` NOT LIKE \"%searchFor%\"", query);
		}

		[Test]
		public async void WhereEndsWithString()
		{
			var query = await GetQueryString(context =>
				context.Domains["TestDomain1"].Items.Where(i =>
					i["TestAtt1"].EndsWith("searchFor")));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` LIKE \"%searchFor\"", query);
		}

		[Test]
		public async void WhereNotEndsWithString()
		{
			var query = await GetQueryString(context =>
				context.Domains["TestDomain1"].Items.Where(i =>
					!i["TestAtt1"].EndsWith("searchFor")));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` NOT LIKE \"%searchFor\"", query);
		}

		[Test]
		public async void WhereStartsWithString()
		{
			var query = await GetQueryString(context =>
				context.Domains["TestDomain1"].Items.Where(i =>
					i["TestAtt1"].StartsWith("searchFor")));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` LIKE \"searchFor%\"", query);
		}

        [Test]
        public async void WhereNotStartsWithString()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                    i["TestAtt1"].StartsWith("searchFor") == false));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` NOT LIKE \"searchFor%\"", query);
        }

        [Test]
        public async void WhereNumberBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                    i["TestAtt1"] == 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"1\"", query);
        }

        [Test]
        public async void WhereItemNameContains()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                    i.Name.Contains("searchFor")));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE itemName() LIKE \"%searchFor%\"", query);
        }

        [Test]
        public async void OrderByBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items.OrderBy(i => i["TestAtt1"]));
            Assert.AreEqual("SELECT * FROM `TestDomain1` ORDERBY `TestAtt1` ASC", query);
        }

        [Test]
        public async void OrderByComplex()
        {
            var query = await GetQueryString(context => 
                context.Domains["TestDomain1"].Items
                    .OrderBy(i => i["TestAtt1"])
                    .OrderByDescending(i => i["TestAtt2"]));
            Assert.AreEqual("SELECT * FROM `TestDomain1` ORDERBY `TestAtt1` ASC, `TestAtt2` DESC", query);
        }

        [Test]
        public async void LimitBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                    .Where(i => i["TestAtt1"] > 0)
                    .Take(20));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` > \"0\" LIMIT 20", query);
        }

        [Test]
        public async void InBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                    .Where(i => i["TestAtt1"].In(1, 2, 3)));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` IN( \"1\", \"2\", \"3\" )", query);
        }

        [Test]
        public async void BetweenBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                   .Where(i => i["TestAtt1"].Between(7.5, 50)));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` BETWEEN \"7.5\" AND \"50\"", query);
        }

        [Test]
        public async void Every()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                   .Where(i => i["TestAtt1"].Every() > 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE every( `TestAtt1` ) > \"1\"", query);
        }

        [Test]
        public async void CountBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                    .Count());
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1`", query);
        }

        [Test]
        public async void CountWhere()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                    .Where(i => i["TestAtt1"] > 1)
                    .Count());
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1` WHERE `TestAtt1` > \"1\"", query);
        }

        [Test]
        public async void CountWithPredicate()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                    .Count(i => i["TestAtt1"] > 1));
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1` WHERE `TestAtt1` > \"1\"", query);
        }

        [Test]
        public async void FirstBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .First());
            Assert.AreEqual("SELECT * FROM `TestDomain1` LIMIT 1", query);
        }

        [Test]
        public async void FirstOrDefaultBasic()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .FirstOrDefault());
            Assert.AreEqual("SELECT * FROM `TestDomain1` LIMIT 1", query);
        }

        [Test]
        public async void FirstWithPredicate()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .First(i => i["TestAtt1"] > 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` > \"1\" LIMIT 1", query);
        }

        [Test]
        public async void FirstOrDefaultWithPredicate()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .FirstOrDefault(i => i["TestAtt1"] > 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` > \"1\" LIMIT 1", query);
        }

		[Test]
		public async void IsNull()
		{
			var query = await GetQueryString(context =>
				context.Domains["TestDomain1"].Items
				.Where(i => i["TestAtt1"] == null));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` IS NULL", query);
		}

		[Test]
		public async void IsNotNull()
		{
			var query = await GetQueryString(context =>
				context.Domains["TestDomain1"].Items
				.Where(i => i["TestAtt1"] != null));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` IS NOT NULL", query);
		}

		[Test]
		public async void SelectWithItemName()
		{
			var query = await GetQueryString(context =>
				context.Domains["TestDomain1"].Items
				.Select(i => new {
					Name = i.Name,
					Value = i["Value"]
				}));
			Assert.AreEqual("SELECT itemName(), `Value` FROM `TestDomain1`", query);
		}

        [Test]
        public async void OrderByWithItemNameNotExplicitlyInSelect()
        {
            var query = await GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .OrderBy(i => i.Name)
                .Select(i => new {
                    Value = i["Value"]
                }));
            Assert.AreEqual("SELECT `Value`, itemName() FROM `TestDomain1` ORDERBY itemName() ASC", query);
        }

        private async Task<string> GetQueryString<T>(Func<ISimpleDbContext, T> query)
        {
            string output = null;
            var captureService = new QueryOutputCaptureService(val => output = val);
            using (var context = SimpleDbContext.Create(captureService))
            {
                if (typeof (IQueryable).IsAssignableFrom(typeof(T)))
                {
                    await (query(context) as IQueryable<ISimpleDbItem>).ForEachAsync(o => { });
                }
                else
                {
                    query(context);
                }
            }
            return output;
        }
    }
}
