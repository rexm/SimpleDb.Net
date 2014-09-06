using System.Collections.Specialized;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Transport
{
    internal interface IAwsRestService
    {
        Task<XElement> ExecuteRequestAsync(NameValueCollection arguments);
    }
}