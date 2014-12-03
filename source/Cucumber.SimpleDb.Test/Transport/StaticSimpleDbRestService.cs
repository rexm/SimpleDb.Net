using NUnit.Framework;
using System;
using System.Xml.Linq;
using System.Linq;

namespace Cucumber.SimpleDb.Test.Transport
{
    internal class StaticSimpleDbRestService : ISimpleDbService
    {
        public StaticSimpleDbRestService()
        {
        }
            
        #region ISimpleDbService implementation

        public virtual XElement BatchDeleteAttributes (string domain, params object[] items)
        {
            throw new NotImplementedException ();
        }
        public virtual XElement BatchPutAttributes (string domain, params object[] items)
        {
            throw new NotImplementedException ();
        }
        public virtual XElement CreateDomain (string domain)
        {
            throw new NotImplementedException ();
        }
        public virtual XElement DeleteDomain (string domain)
        {
            throw new NotImplementedException ();
        }
        public virtual XElement DeleteAttributes (string domain, string itemName, params object[] attributes)
        {
            throw new NotImplementedException ();
        }
        public virtual XElement GetDomainMeta (string domain)
        {
            throw new NotImplementedException ();
        }
        public virtual XElement ListDomains ()
        {
            return ListDomains(null);
        }
        public virtual XElement ListDomains (string nextPageToken)
        {
            return new XElement(SdbNs + "ListDomainsResponse", new XElement(SdbNs + "ListDomainsResult", new XElement[]
            {
                new XElement(SdbNs + "DomainName", "pictures"),
                new XElement(SdbNs + "DomainName", "documents"),
                new XElement(SdbNs + "DomainName", "contacts")
            }));
        }
        public virtual XElement PutAttributes (string domain, string name, params object[] attributes)
        {
            throw new NotImplementedException ();
        }
        public virtual XElement GetAttributes (string domain, string name, bool useConsistency, params string[] attributeNames)
        {
            return new XElement(SdbNs + "SelectResponse",
                new XElement(SdbNs + "SelectResult", Enumerable.Range(1, 3).Select(i => GenerateElement(i))),
                new XElement(SdbNs + "ResponseMetadata", 
                    new XElement(SdbNs + "RequestId", Guid.NewGuid()),
                    new XElement(SdbNs + "BoxUsage", 0.001m)));
        }
        public virtual XElement Select (string query, bool useConsistency)
        {
            return Select(query, useConsistency, null);
        }
        public virtual XElement Select (string query, bool useConsistency, string nextPageToken)
        {
            return new XElement(SdbNs + "SelectResponse", new XElement(SdbNs + "SelectResult", new XElement[]
            {
                new XElement(SdbNs + "Item", new XElement[]{
                    new XElement(SdbNs + "Name", "ItemName1"),
                    new XElement(SdbNs + "Attribute", new XElement[] {
                        new XElement(SdbNs + "Name", "Count"),
                        new XElement(SdbNs + "Value", 1)
                    })
                })
            }));
        }

        #endregion

        private readonly static XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";

        private XElement GenerateElement(int index)
        {
            return new XElement(SdbNs + "Item", new XElement[]
                {
                    new XElement(SdbNs + "Name", "ItemName" + index),
                    new XElement(SdbNs + "Attribute", new XElement[]
                        {
                            new XElement(SdbNs + "Name", "Att1"),
                            new XElement(SdbNs + "Value", new Random().Next())
                        }),
                    new XElement(SdbNs + "Attribute", new XElement[]
                        {
                            new XElement(SdbNs + "Name", "Att2"),
                            new XElement(SdbNs + "Value", new Random().Next())
                        })
                });
        }
    }
}

