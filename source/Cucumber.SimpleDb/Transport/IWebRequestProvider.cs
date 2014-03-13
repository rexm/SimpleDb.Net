using System;
using System.Net;

namespace Cucumber.SimpleDb.Transport
{
    internal interface IWebRequestProvider
    {
        WebRequest Create(string uri);
    }
}

