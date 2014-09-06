using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;

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

        private async Task<XElement> UpdateStatistics(Task<XElement> response)
        {
            var result = await response;
            var metadata = result
                .Element(sdbNs + "ResponseMetadata");
            if (metadata != null)
            {
                UpdateUsage(metadata);
                UpdateId(metadata);
                UpdateCount(metadata);
            }
            return response.Result;
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

        Task<XElement> ISimpleDbService.BatchDeleteAttributesAsync(string domain, params object[] items)
        {
            return UpdateStatistics(_service.BatchDeleteAttributesAsync(domain, items));
        }

        Task<XElement> ISimpleDbService.BatchPutAttributesAsync(string domain, params object[] items)
        {
            return UpdateStatistics(_service.BatchPutAttributesAsync(domain, items));
        }

        Task<XElement> ISimpleDbService.CreateDomainAsync(string domain)
        {
            return UpdateStatistics(_service.CreateDomainAsync(domain));
        }

        Task<XElement> ISimpleDbService.DeleteDomainAsync(string domain)
        {
            return UpdateStatistics(_service.DeleteDomainAsync(domain));
        }

        Task<XElement> ISimpleDbService.DeleteAttributesAsync(string domain, string itemName, params object[] attributes)
        {
            return UpdateStatistics(_service.DeleteAttributesAsync(domain, itemName, attributes));
        }

        Task<XElement> ISimpleDbService.GetDomainMetaAsync(string domain)
        {
            return UpdateStatistics(_service.GetDomainMetaAsync(domain));
        }

        Task<XElement> ISimpleDbService.ListDomainsAsync()
        {
            return UpdateStatistics(_service.ListDomainsAsync());
        }

        Task<XElement> ISimpleDbService.ListDomainsAsync(string nextPageToken)
        {
            return UpdateStatistics(_service.ListDomainsAsync(nextPageToken));
        }

        Task<XElement> ISimpleDbService.PutAttributesAsync(string domain, string name, params object[] attributes)
        {
            return UpdateStatistics(_service.PutAttributesAsync(domain, name, attributes));
        }

        Task<XElement> ISimpleDbService.GetAttributesAsync(string domain, string name, bool useConsistency, params string[] attributeNames)
        {
            return UpdateStatistics(_service.GetAttributesAsync(domain, name, useConsistency, attributeNames));
        }

        Task<XElement> ISimpleDbService.SelectAsync(string query, bool useConsistency)
        {
            return UpdateStatistics(_service.SelectAsync(query, useConsistency));
        }

        Task<XElement> ISimpleDbService.SelectAsync(string query, bool useConsistency, string nextPageToken)
        {
            return UpdateStatistics(_service.SelectAsync(query, useConsistency, nextPageToken));
        }
	}

}
