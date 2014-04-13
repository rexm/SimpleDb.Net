using System.Collections.Specialized;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Async.Transport
{
    internal interface IAwsRestService
    {
        XElement ExecuteRequest(NameValueCollection arguments);
    }
}