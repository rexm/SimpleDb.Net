using NUnit.Framework;
using System;
using Cucumber.SimpleDb;
using Cucumber.SimpleDb.Transport;

namespace Cucumber.SimpleDb.Test.Statistics
{
    [TestFixture]
    public class BasicStatisticsTests
    {
        [Test]
        public void TestOneOperation()
        {
            var proxy = new StatisticsCollectorProxy(new SimpleDbRestService(new SimpleInMemoryItemService(1)));
            Assert.AreEqual(0, proxy.LastOperationUsage);
            Assert.AreEqual(0, proxy.TotalContextUsage);
            Assert.AreEqual(0, proxy.OperationCount);
            Assert.IsNull(proxy.LastOperationId);
            ((ISimpleDbService)proxy).Select("", false);
            Assert.AreEqual(0.001m, proxy.LastOperationUsage);
            Assert.AreEqual(0.001m, proxy.TotalContextUsage);
            Assert.AreEqual(1, proxy.OperationCount);
            Assert.IsNotNullOrEmpty(proxy.LastOperationId);
        }

        [Test]
        public void TestTwoOperations()
        {
            var proxy = new StatisticsCollectorProxy(new SimpleDbRestService(new SimpleInMemoryItemService(1)));
            Assert.AreEqual(0, proxy.LastOperationUsage);
            Assert.AreEqual(0, proxy.TotalContextUsage);
            Assert.AreEqual(0, proxy.OperationCount);
            Assert.IsNull(proxy.LastOperationId);
            ((ISimpleDbService)proxy).Select("", false);
            Assert.AreEqual(0.001m, proxy.LastOperationUsage);
            Assert.AreEqual(0.001m, proxy.TotalContextUsage);
            Assert.AreEqual(1, proxy.OperationCount);
            Assert.IsNotNullOrEmpty(proxy.LastOperationId);
            ((ISimpleDbService)proxy).Select("", false);
            Assert.AreEqual(0.001m, proxy.LastOperationUsage);
            Assert.AreEqual(0.002m, proxy.TotalContextUsage);
            Assert.AreEqual(2, proxy.OperationCount);
            Assert.IsNotNullOrEmpty(proxy.LastOperationId);
        }
    }
}

