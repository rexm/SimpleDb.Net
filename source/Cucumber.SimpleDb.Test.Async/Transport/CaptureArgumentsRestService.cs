using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cucumber.SimpleDb.Async.Transport;

namespace Cucumber.SimpleDb.Test.Async.Transport
{
    public class CaptureArgumentsRestService : IAwsRestService
    {
        private readonly Action<XElement> _captureResult;

        public CaptureArgumentsRestService(Action<XElement> captureResult)
        {
            _captureResult = captureResult;
        }

        public Task<XElement> ExecuteRequestAsync(NameValueCollection arguments)
        {
            var doc = new XDocument(
                new XElement("Arguments",
                    arguments.OfType<string>().Select(key =>
                        new XElement("Argument",
                            new XElement("Key", key),
                            new XElement("Value", arguments[key])))
                        .OfType<object>().ToArray()));
            _captureResult(doc.Root);
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