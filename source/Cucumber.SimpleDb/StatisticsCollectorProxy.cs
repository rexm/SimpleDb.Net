using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cucumber.SimpleDb
{
    internal class StatisticsCollectorProxy : ISimpleDbStatistics, ISimpleDbService
	{
        private static readonly XNamespace sdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly ISimpleDbService _service;

        public StatisticsCollectorProxy(ISimpleDbService service)
        {
            _service = service;
        }

        public decimal LastOperationUsage { get; private set; }

        public decimal TotalContextUsage { get; private set; }

        public string LastOperationId { get; private set; }

        public int OperationCount { get; private set; }

        private XElement UpdateStatistics(XElement response)
        {
            var metadata = response
                .Element(sdbNs + "ResponseMetadata");
            if (metadata != null)
            {
                UpdateUsage(metadata);
                UpdateId(metadata);
                UpdateCount(metadata);
            }
            return response;
        }

        private void UpdateUsage(XElement metadata)
        {
            var boxUsageElement = metadata
                .Element(sdbNs + "BoxUsage");
            if (boxUsageElement == null)
            {
                return;
            }
            decimal boxUsage;
            if (decimal.TryParse(boxUsageElement.Value, out boxUsage))
            {
                LastOperationUsage = boxUsage;
                TotalContextUsage += boxUsage;
            }
        }

        private void UpdateId(XElement metadata)
        {
            var requestIdElement = metadata
                .Element(sdbNs + "RequestId");
            if (requestIdElement == null)
            {
                return;
            }
            LastOperationId = requestIdElement.Value;
        }

        private void UpdateCount(XElement metadata)
        {
            OperationCount++;
        }

        XElement ISimpleDbService.BatchDeleteAttributes(string domain, params object[] items)
        {
            return UpdateStatistics(_service.BatchDeleteAttributes(domain, items));
        }

        XElement ISimpleDbService.BatchPutAttributes(string domain, params object[] items)
        {
            return UpdateStatistics(_service.BatchPutAttributes(domain, items));
        }

        XElement ISimpleDbService.CreateDomain(string domain)
        {
            return UpdateStatistics(_service.CreateDomain(domain));
        }

        XElement ISimpleDbService.DeleteDomain(string domain)
        {
            return UpdateStatistics(_service.DeleteDomain(domain));
        }

        XElement ISimpleDbService.DeleteAttributes(string domain, string itemName, params object[] attributes)
        {
            return UpdateStatistics(_service.DeleteAttributes(domain, itemName, attributes));
        }

        XElement ISimpleDbService.GetDomainMeta(string domain)
        {
            return UpdateStatistics(_service.GetDomainMeta(domain));
        }

        XElement ISimpleDbService.ListDomains()
        {
            return UpdateStatistics(_service.ListDomains());
        }

        XElement ISimpleDbService.ListDomains(string nextPageToken)
        {
            return UpdateStatistics(_service.ListDomains(nextPageToken));
        }

        XElement ISimpleDbService.PutAttributes(string domain, string name, params object[] attributes)
        {
            return UpdateStatistics(_service.PutAttributes(domain, name, attributes));
        }

        XElement ISimpleDbService.GetAttributes(string domain, string name, bool useConsistency, params string[] attributeNames)
        {
            return UpdateStatistics(_service.GetAttributes(domain, name, useConsistency, attributeNames));
        }

        XElement ISimpleDbService.Select(string query, bool useConsistency)
        {
            return UpdateStatistics(_service.Select(query, useConsistency));
        }

        XElement ISimpleDbService.Select(string query, bool useConsistency, string nextPageToken)
        {
            return UpdateStatistics(_service.Select(query, useConsistency, nextPageToken));
        }
	}

}
