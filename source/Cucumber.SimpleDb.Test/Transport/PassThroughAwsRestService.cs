using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Test
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
            return new Task<XElement>(() => doc.Root);
        }
    }
}