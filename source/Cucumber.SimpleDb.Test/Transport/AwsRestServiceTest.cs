using System.Net;
using System.Net.Http;
using Cucumber.SimpleDb.Test.Fakes;
using NUnit.Framework;
using System.Collections.Specialized;
using Cucumber.SimpleDb.Transport;

namespace Cucumber.SimpleDb.Test
{
		
    [TestFixture ()]
    public class AwsRestServiceTest
    {
				private HttpRequestMessage _requestMessage;
				private HttpClient _httpClient;
				[SetUp]
				public void Setup()
				{
						var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
						{
							Content = new StringContent("<All><Ok/></All>")
						};
					_httpClient = new HttpClient(new FakeHttpMessageHandler(responseMessage, req => _requestMessage = req));
							
				}

        [Test ()]
        public void ValidateRequestSignature ()
        {
            string generatedUri = null;
						var service = new AwsRestService("myPublicKey", "myPrivateKey", _httpClient);
            service.ExecuteRequest (new NameValueCollection {
                { "Item.0.ItemName", "TestItem1" },
                { "Item.0.Attribute.0.Name", "TestAtt1" },
                { "Item.0.Attribute.0.Value", "123" }
            });
						Assert.NotNull(_requestMessage);
        }
    }
}

