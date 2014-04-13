using System.Collections.Specialized;
using System.Net.Http;
using Cucumber.SimpleDb.Async.Transport;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test.Async.Transport
{
    [TestFixture]
    public class AwsRestServiceTest
    {
        [Test]
        public void ValidateRequestSignature()
        {
            string generatedUri = null;
            var service = new AwsRestService("myPublicKey", "myPrivateKey", new HttpClient());
            service.ExecuteRequestAsync(new NameValueCollection
            {
                {
                    "Item.0.ItemName", "TestItem1"
                },
                {
                    "Item.0.Attribute.0.Name", "TestAtt1"
                },
                {
                    "Item.0.Attribute.0.Value", "123"
                }
            });
            //TODO: validate HMAC
        }
    }
}