using System.Collections.Specialized;
using Cucumber.SimpleDb.Transport;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class AwsRestServiceTest
    {
        [Test]
        public void ValidateRequestSignature()
        {
            string generatedUri = null;
            var service = new AwsRestService("myPublicKey", "myPrivateKey", new CaptureUriRequestProvider(uri => generatedUri = uri));
            service.ExecuteRequest(new NameValueCollection
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