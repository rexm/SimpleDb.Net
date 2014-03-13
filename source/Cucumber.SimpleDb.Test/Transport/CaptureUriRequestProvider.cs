using System;
using System.Net;
using System.IO;
using System.Text;
using Cucumber.SimpleDb.Transport;

namespace Cucumber.SimpleDb.Test
{
    public class CaptureUriRequestProvider : IWebRequestProvider
    {
        private readonly Action<string> _captureUri;

        public CaptureUriRequestProvider (Action<string> captureUri)
        {
            _captureUri = captureUri;
        }
            
        public System.Net.WebRequest Create (string uri)
        {
            _captureUri (uri);
            return new DummyWebRequest();
        }

        private class DummyWebRequest : WebRequest
        {
            public override WebResponse GetResponse ()
            {
                return new DummyWebResponse ();
            }

            private class DummyWebResponse : WebResponse
            {
                public override Stream GetResponseStream ()
                {
                    return new MemoryStream (Encoding.UTF8.GetBytes ("<Empty />"));
                }

                public override void Close ()
                {
                }
            }
        }
    }
}

