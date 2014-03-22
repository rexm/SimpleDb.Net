using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;

namespace Cucumber.SimpleDb.Test
{
    public class PassThroughAwsRestService : IAwsRestService
    {
        public PassThroughAwsRestService()
        {
        }

        public XElement ExecuteRequest (NameValueCollection arguments)
        {
            var doc = new XDocument (
                  new XElement ("Arguments", 
                              arguments.OfType<string> ().Select (key =>
                        new XElement ("Argument",
                                  new XElement ("Key", key),
                                  new XElement ("Value", arguments [key])))
                    .OfType<object> ().ToArray ()));
            return doc.Root;
        }
    }
}

