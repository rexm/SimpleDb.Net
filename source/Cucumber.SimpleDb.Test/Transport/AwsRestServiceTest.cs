using System.Linq;
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

        [Test]
        public void ExecuteRequest_ShouldSerializeArgumentsToRequest ()
        {
						var service = new AwsRestService("myPublicKey", "myPrivateKey", _httpClient);
						var arguments = new NameValueCollection {
                { "Item.0.ItemName", "TestItem1" },
                { "Item.0.Attribute.0.Name", "TestAtt1" },
                { "Item.0.Attribute.0.Value", "123" }
            };
            service.ExecuteRequest(arguments);
						Assert.NotNull(_requestMessage);

						var requestData = _requestMessage.RequestUri.ToNameValueCollection();
						Assert.NotNull(requestData);
						Assert.IsTrue(requestData.Count > 0);
						foreach(string key in arguments)
						{
							Assert.AreEqual(arguments[key], requestData[key]);	
						}
						
        }

				[Test]
				public void ExecuteRequest_ShouldAddStandardArgumentsToRequest()
				{
					var service = new AwsRestService("myPublicKey", "myPrivateKey", _httpClient);
					service.ExecuteRequest(new NameValueCollection());
					Assert.NotNull(_requestMessage);

					var requestData = _requestMessage.RequestUri.ToNameValueCollection();
					Assert.NotNull(requestData);
					Assert.IsTrue(requestData.Count > 0);
					new[]
					{
						"AWSAccessKeyId",
						"SignatureVersion",
						"SignatureMethod",
						"Timestamp",
						"Version"
					}.ToList()
					.ForEach(key => Assert.IsNotNullOrEmpty(requestData[key]));
					Assert.AreEqual("myPublicKey", requestData["AWSAccessKeyId"]);
				}

				[Test]
				public void ExecuteRequest_ShouldAddSignatureToRequest()
				{
					var service = new AwsRestService("myPublicKey", "myPrivateKey", _httpClient);
					service.ExecuteRequest(new NameValueCollection());
					Assert.NotNull(_requestMessage);

					var requestData = _requestMessage.RequestUri.ToNameValueCollection();
					Assert.NotNull(requestData);
					Assert.IsTrue(requestData.Count > 0);
					Assert.IsNotNullOrEmpty(requestData["Signature"]);

					//TODO: (CV) Should assert the actual signature. Need to move the signature generation logic out of the AwsRequestService class
				}
    }
}

