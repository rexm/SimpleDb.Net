using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cucumber.SimpleDb.Async;

namespace Cucumber.SimpleDb.Test.Async.LINQ.IntegrationTests
{
    internal class QueryOutputCaptureService : ISimpleDbService
    {
        private readonly Action<string> _output;

        public QueryOutputCaptureService(Action<string> output)
        {
            _output = output;
        }

        public Task<XElement> BatchDeleteAttributesAsync(string domain, params object[] items)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> BatchPutAttributesAsync(string domain, params object[] items)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> CreateDomainAsync(string domain)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> DeleteDomainAsync(string domain)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> DeleteAttributesAsync(string domain, string itemName, params object[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> GetDomainMetaAsync(string domain)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> ListDomainsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<XElement> ListDomainsAsync(string nextPageToken)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> PutAttributesAsync(string domain, string name, params object[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> GetAttributesAsync(string domain, string name, bool useConsistency, params string[] attributeNames)
        {
            throw new NotImplementedException();
        }

        public Task<XElement> SelectAsync(string query, bool useConsistency)
        {
            return SelectAsync(query, useConsistency, null);
        }

        public Task<XElement> SelectAsync(string query, bool useConsistency, string nextPageToken)
        {
            _output(query);
            XNamespace ns = "http://sdb.amazonaws.com/doc/2009-04-15/";
            return Task.FromResult(new XElement(ns + "SelectResponse", new XElement(ns + "SelectResult", new[]
            {
                new XElement(ns + "Item", new[]
                {
                    new XElement(ns + "Name", "ItemName1"),
                    new XElement(ns + "Attribute", new[]
                    {
                        new XElement(ns + "Name", "Count"),
                        new XElement(ns + "Value", 1)
                    })
                })
            })));
        }
    }
}