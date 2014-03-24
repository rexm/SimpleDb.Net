using System.Net;

namespace Cucumber.SimpleDb.Async.Transport
{
    internal interface IWebRequestProvider
    {
        WebRequest Create(string uri);
    }
}