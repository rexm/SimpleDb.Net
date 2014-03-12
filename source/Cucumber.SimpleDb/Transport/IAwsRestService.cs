using System;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace Cucumber.SimpleDb
{
    public interface IAwsRestService
    {
        XElement ExecuteRequest (NameValueCollection arguments);
    }
}

