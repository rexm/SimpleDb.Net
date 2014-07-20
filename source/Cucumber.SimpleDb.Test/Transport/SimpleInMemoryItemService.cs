using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;

namespace Cucumber.SimpleDb.Test
{
    public class SimpleInMemoryItemService : IAwsRestService
    {
        private static readonly XNamespace ns = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly int _items;

        public SimpleInMemoryItemService(int items)
        {
            _items = items;
        }

        public XElement ExecuteRequest(NameValueCollection arguments)
        {

            return new XElement(ns + "SelectResponse", new XElement(ns + "SelectResult", Enumerable.Range(1, _items).Select(i => GenerateElement(i))));
        }

        private XElement GenerateElement(int index)
        {
            return new XElement(ns + "Item", new XElement[]
                {
                    new XElement(ns + "Name", "ItemName" + index),
                    new XElement(ns + "Attribute", new XElement[]
                        {
                            new XElement(ns + "Name", "Att1"),
                            new XElement(ns + "Value", new Random().Next())
                        }),
                    new XElement(ns + "Attribute", new XElement[]
                        {
                            new XElement(ns + "Name", "Att2"),
                            new XElement(ns + "Value", new Random().Next())
                        })
                });
        }
    }
}

