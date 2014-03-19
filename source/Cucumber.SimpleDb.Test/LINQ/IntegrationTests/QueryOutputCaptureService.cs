using System;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Test
{
    internal class QueryOutputCaptureService : ISimpleDbService
    {
        private readonly Action<string> _output;

        public QueryOutputCaptureService(Action<string> output)
        {
            _output = output;
        }

        #region ISimpleDbService implementation

        public XElement BatchDeleteAttributes (string domain, params object[] items)
        {
            throw new NotImplementedException ();
        }
        public XElement BatchPutAttributes (string domain, params object[] items)
        {
            throw new NotImplementedException ();
        }
        public XElement CreateDomain (string domain)
        {
            throw new NotImplementedException ();
        }
        public XElement DeleteDomain (string domain)
        {
            throw new NotImplementedException ();
        }
        public XElement DeleteAttributes (string domain, string itemName, params object[] attributes)
        {
            throw new NotImplementedException ();
        }
        public XElement GetDomainMeta (string domain)
        {
            throw new NotImplementedException ();
        }
        public XElement ListDomains ()
        {
            throw new NotImplementedException ();
        }
        public XElement ListDomains (string nextPageToken)
        {
            throw new NotImplementedException ();
        }
        public XElement PutAttributes (string domain, string name, params object[] attributes)
        {
            throw new NotImplementedException ();
        }
        public XElement GetAttributes (string domain, string name, bool useConsistency, params string[] attributeNames)
        {
            throw new NotImplementedException ();
        }
        public XElement Select (string query, bool useConsistency)
        {
            return Select(query, useConsistency, null);
        }
        public XElement Select (string query, bool useConsistency, string nextPageToken)
        {
            _output(query);
            XNamespace ns = "http://sdb.amazonaws.com/doc/2009-04-15/";
            return new XElement(ns + "SelectResponse", new XElement(ns + "SelectResult", new XElement[]
                {
                    new XElement(ns + "Item", new XElement[]{
                        new XElement(ns + "Name", "ItemName1"),
                        new XElement(ns + "Attribute", new XElement[] {
                            new XElement(ns + "Name", "Count"),
                            new XElement(ns + "Value", 1)
                        })
                    })
                }));
        }

        #endregion
    }
}

