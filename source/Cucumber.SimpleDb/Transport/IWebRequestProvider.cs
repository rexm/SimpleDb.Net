using System;
using System.Net;

namespace Cucumber.SimpleDb
{
    internal interface IWebRequestProvider
    {
        WebRequest Create(string uri);
    }
}

