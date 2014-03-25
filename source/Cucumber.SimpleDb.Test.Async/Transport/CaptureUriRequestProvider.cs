namespace Cucumber.SimpleDb.Test.Async.Transport
{
    //public class CaptureUriRequestProvider : IWebRequestProvider
    //{
    //    private readonly Action<string> _captureUri;

    //    public CaptureUriRequestProvider(Action<string> captureUri)
    //    {
    //        _captureUri = captureUri;
    //    }

    //    public WebRequest Create(string uri)
    //    {
    //        _captureUri(uri);
    //        return new DummyWebRequest();
    //    }

    //    private class DummyWebRequest : WebRequest
    //    {
    //        public override WebResponse GetResponse()
    //        {
    //            return new DummyWebResponse();
    //        }

    //        private class DummyWebResponse : WebResponse
    //        {
    //            public override Stream GetResponseStream()
    //            {
    //                return new MemoryStream(Encoding.UTF8.GetBytes("<Empty />"));
    //            }

    //            public override void Close()
    //            {
    //            }
    //        }
    //    }
    //}
}