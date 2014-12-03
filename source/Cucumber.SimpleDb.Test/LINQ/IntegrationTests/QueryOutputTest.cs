using System;
using System.Linq;
using NUnit.Framework;
using Cucumber.SimpleDb.Test.Transport;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture()]
    public class QueryOutputTest
    {
        [Test]
        public void ImplicitSelect()
        {
            var query = GetQueryString (context =>
                context.Domains["TestDomain1"].Items);
            Assert.AreEqual("SELECT * FROM `TestDomain1`", query);
        }

        [Test]
        public void ExplicitSelect()
        {
            var query = GetQueryString (context =>
                context.Domains["TestDomain1"].Items
                .Select(i => new
                    {
                        Att1 = i["Att1"],
                        Att2 = i["Att2"]
                    }));
            Assert.AreEqual("SELECT `Att1`, `Att2` FROM `TestDomain1`", query);
        }

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
                   i["TestAtt1"].Contains("searchFor")));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` LIKE \"%searchFor%\"", query);
        }

		[Test]
		public void WhereNotContainsString()
		{
			var query = GetQueryString(context =>
				context.Domains["TestDomain1"].Items.Where(i =>
					!i["TestAtt1"].Contains("searchFor")));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` NOT LIKE \"%searchFor%\"", query);
		}

		[Test]
		public void WhereEndsWithString()
		{
			var query = GetQueryString(context =>
				context.Domains["TestDomain1"].Items.Where(i =>
					i["TestAtt1"].EndsWith("searchFor")));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` LIKE \"%searchFor\"", query);
		}

		[Test]
		public void WhereNotEndsWithString()
		{
			var query = GetQueryString(context =>
				context.Domains["TestDomain1"].Items.Where(i =>
					!i["TestAtt1"].EndsWith("searchFor")));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` NOT LIKE \"%searchFor\"", query);
		}

		[Test]
		public void WhereStartsWithString()
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
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                               i["TestAtt1"] == 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` = \"1\"", query);
        }

        [Test]
        public void WhereItemNameContains()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items.Where(i =>
                    i.Name.Contains("searchFor")));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE itemName() LIKE \"%searchFor%\"", query);
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
            var query = GetQueryString(context => 
                context.Domains["TestDomain1"].Items
                   .OrderBy(i => i["TestAtt1"])
                    .OrderByDescending(i => i["TestAtt2"]));
            Assert.AreEqual("SELECT * FROM `TestDomain1` ORDERBY `TestAtt1` ASC, `TestAtt2` DESC", query);
        }

        [Test]
        public void LimitBasic()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                   .Where(i => i["TestAtt1"] > 0)
                   .Take(20));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` > \"0\" LIMIT 20", query);
        }

        [Test]
        public void InBasic()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                   .Where(i => i["TestAtt1"].In(1,2,3)));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` IN( \"1\", \"2\", \"3\" )", query);
        }

        [Test]
        public void BetweenBasic()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                   .Where(i => i["TestAtt1"].Between (7.5, 50)));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` BETWEEN \"7.5\" AND \"50\"", query);
        }

        [Test]
        public void Every()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                   .Where(i => i["TestAtt1"].Every () > 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE every( `TestAtt1` ) > \"1\"", query);
        }

        [Test]
        public void CountBasic()
        {
            var query = GetQueryString (context =>
                context.Domains["TestDomain1"].Items
                .Count());
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1`", query);
        }

        [Test]
        public void CountWhere()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .Where(i => i["TestAtt1"] > 1)
                .Count());
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1` WHERE `TestAtt1` > \"1\"", query);
        }

        [Test]
        public void CountWithPredicate()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .Count(i => i["TestAtt1"] > 1));
            Assert.AreEqual("SELECT COUNT(*) FROM `TestDomain1` WHERE `TestAtt1` > \"1\"", query);
        }

        [Test]
        public void FirstBasic()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .First());
            Assert.AreEqual("SELECT * FROM `TestDomain1` LIMIT 1", query);
        }

        [Test]
        public void FirstOrDefaultBasic()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .FirstOrDefault());
            Assert.AreEqual("SELECT * FROM `TestDomain1` LIMIT 1", query);
        }

        [Test]
        public void FirstWithPredicate()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .First(i => i["TestAtt1"] > 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` > \"1\" LIMIT 1", query);
        }

        [Test]
        public void FirstOrDefaultWithPredicate()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .FirstOrDefault(i => i["TestAtt1"] > 1));
            Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` > \"1\" LIMIT 1", query);
        }

		[Test]
		public void IsNull()
		{
			var query = GetQueryString(context =>
				context.Domains["TestDomain1"].Items
				.Where(i => i["TestAtt1"] == null));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` IS NULL", query);
		}

		[Test]
		public void IsNotNull()
		{
			var query = GetQueryString(context =>
				context.Domains["TestDomain1"].Items
				.Where(i => i["TestAtt1"] != null));
			Assert.AreEqual("SELECT * FROM `TestDomain1` WHERE `TestAtt1` IS NOT NULL", query);
		}

		[Test]
		public void SelectWithItemName()
		{
			var query = GetQueryString(context =>
				context.Domains["TestDomain1"].Items
				.Select(i => new {
					Name = i.Name,
					Value = i["Value"]
				}));
			Assert.AreEqual("SELECT itemName(), `Value` FROM `TestDomain1`", query);
		}

        [Test]
        public void OrderByWithItemNameNotExplicitlyInSelect()
        {
            var query = GetQueryString(context =>
                context.Domains["TestDomain1"].Items
                .OrderBy(i => i.Name)
                .Select(i => new {
                    Value = i["Value"]
                }));
            Assert.AreEqual("SELECT `Value`, itemName() FROM `TestDomain1` ORDERBY itemName() ASC", query);
        }

        private string GetQueryString<T>(Func<ISimpleDbContext, T> query)
        {
            string output = null;
            var captureService = new QueryOutputCaptureService(val => output = val);
            using(var context = SimpleDbContext.Create(captureService))
            {
                if (typeof(IQueryable).IsAssignableFrom(typeof(T)))
                {
                    foreach (var item in query(context) as IQueryable)
                    {
                        //no results
                    }
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

