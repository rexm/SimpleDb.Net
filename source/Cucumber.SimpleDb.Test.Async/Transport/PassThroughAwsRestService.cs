using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cucumber.SimpleDb.Async.Transport;

namespace Cucumber.SimpleDb.Test.Async.Transport
{
    public class PassThroughAwsRestService : IAwsRestService
    {
        public Task<XElement> ExecuteRequestAsync(NameValueCollection arguments)
        {
            var doc = new XDocument(
                new XElement("Arguments",
                    arguments.OfType<string>().Select(key =>
                        new XElement("Argument",
                            new XElement("Key", key),
                            new XElement("Value", arguments[key])))
                        .OfType<object>().ToArray()));
            return Task.FromResult(doc.Root);
        }
    }
}