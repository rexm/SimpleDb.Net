using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Async.Transport
{
    internal interface IAwsRestService
    {
        Task<XElement> ExecuteRequestAsync(NameValueCollection arguments);
    }
}