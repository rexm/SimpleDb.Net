using System.Net;

namespace Cucumber.SimpleDb.Transport
{
    internal class WebRequestProvider : IWebRequestProvider
    {
        public WebRequest Create(string uri)
        {
            return WebRequest.Create(uri);
        }
    }
}