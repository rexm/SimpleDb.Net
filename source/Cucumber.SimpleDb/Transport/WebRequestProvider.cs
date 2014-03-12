using System;
using System.Net;

namespace Cucumber.SimpleDb
{
    public class WebRequestProvider : IWebRequestProvider
    {
        public System.Net.WebRequest Create (string uri)
        {
            return HttpWebRequest.Create (uri);
        }
    }
}

