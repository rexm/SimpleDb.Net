using System.Net;

namespace Cucumber.SimpleDb.Async.Transport
{
    internal class WebRequestProvider : IWebRequestProvider
    {
        public WebRequest Create(string uri)
        {
            return WebRequest.Create(uri);
        }
    }
}