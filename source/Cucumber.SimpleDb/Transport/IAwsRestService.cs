using System.Collections.Specialized;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Transport
{
    internal interface IAwsRestService
    {
        XElement ExecuteRequest(NameValueCollection arguments);
    }
}