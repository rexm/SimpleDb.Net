using NUnit.Framework;
using System;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Test.Transport
{
    internal class StaticSimpleDbRestService : ISimpleDbService
    {
        public StaticSimpleDbRestService()
        {
        }

        #region ISimpleDbService implementation

        public virtual Task<XElement> BatchDeleteAttributesAsync(string domain, params object[] items)
        {
            throw new NotImplementedException ();
        }
        public virtual Task<XElement> BatchPutAttributesAsync(string domain, params object[] items)
        {
            throw new NotImplementedException ();
        }
        public virtual Task<XElement> CreateDomainAsync(string domain)
        {
            throw new NotImplementedException ();
        }
        public virtual Task<XElement> DeleteDomainAsync(string domain)
        {
            throw new NotImplementedException ();
        }
        public virtual Task<XElement> DeleteAttributesAsync(string domain, string itemName, params object[] attributes)
        {
            throw new NotImplementedException ();
        }
        public virtual Task<XElement> GetDomainMetaAsync(string domain)
        {
            throw new NotImplementedException ();
        }
        public virtual Task<XElement> ListDomainsAsync()
        {
            return ListDomainsAsync(null);
        }
        public virtual Task<XElement> ListDomainsAsync(string nextPageToken)
        {
            return new Task<XElement>(() => 
                new XElement(SdbNs + "ListDomainsResponse", new XElement(SdbNs + "ListDomainsResult", new XElement[]
                {
                    new XElement(SdbNs + "DomainName", "pictures"),
                    new XElement(SdbNs + "DomainName", "documents"),
                    new XElement(SdbNs + "DomainName", "contacts")
                    })));
        }
        public virtual Task<XElement> PutAttributesAsync(string domain, string name, params object[] attributes)
        {
            throw new NotImplementedException ();
        }
        public virtual Task<XElement> GetAttributesAsync(string domain, string name, bool useConsistency, params string[] attributeNames)
        {
            throw new NotImplementedException ();
        }
        public virtual Task<XElement> SelectAsync(string query, bool useConsistency)
        {
            return SelectAsync(query, useConsistency, null);
        }
        public virtual async Task<XElement> SelectAsync(string query, bool useConsistency, string nextPageToken)
        {
            return await new Task<XElement>(() => 
                new XElement(SdbNs + "SelectResponse", new XElement(SdbNs + "SelectResult", new XElement[]
                {
                    new XElement(SdbNs + "Item", new XElement[]{
                        new XElement(SdbNs + "Name", "ItemName1"),
                        new XElement(SdbNs + "Attribute", new XElement[] {
                            new XElement(SdbNs + "Name", "Count"),
                            new XElement(SdbNs + "Value", 1)
                        })
                    })
                }))
            ).ConfigureAwait(false);
        }

        #endregion

        private readonly static XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
    }
}

