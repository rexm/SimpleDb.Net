using System;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Xml;
using System.Net;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cucumber.SimpleDb.Transport
{
    internal class AwsRestService : IAwsRestService
    {
        private readonly string _publicKey;
        private readonly string _privateKey;
        private readonly IWebRequestProvider _webRequest;

        public AwsRestService(string publicKey, string privateKey, IWebRequestProvider webRequest)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
            _webRequest = webRequest;
        }

        public XElement ExecuteRequest(NameValueCollection arguments)
        {
            arguments = AddStandardArguments(arguments);
            string argumentString = WriteQueryString(arguments);
            var request = _webRequest.Create(string.Format(
                "{0}://{1}/?{2}",
                simpleDbProtocol,
                simpleDbUrl,
                argumentString));
            try
            {
                using (var response = request.GetResponse ())
                {
                    return ProcessAwsResponse (response);
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                var errors = ProcessAwsResponse (response);
                throw new SimpleDbException(string.Format(
                    "Error {0} {1}: AWS returned the following:\n{2}",
                    (int)response.StatusCode,
                    response.StatusCode,
                    string.Join("\n", errors.Descendants("Error")
                        .Select(error => string.Format("{0}: {1}",
                            error.Element("Code").Value,
                            error.Element("Message").Value)))),
                    ex);
            }
        }

        private static XElement ProcessAwsResponse (WebResponse response)
        {
            using (var stream = response.GetResponseStream ())
            {
                try
                {
                    return XDocument.Load (stream).Root;
                }
                catch (XmlException xmlex)
                {
                    throw new SimpleDbException ("AWS returned invalid XML", xmlex);
                }
                catch (InvalidOperationException invalidex)
                {
                    throw new SimpleDbException ("AWS returned invalid XML", invalidex);
                }
            }
        }

        private NameValueCollection AddStandardArguments(NameValueCollection arguments)
        {
            var newArguments = new NameValueCollection(arguments);
            newArguments.Add("AWSAccessKeyId", _publicKey);
            newArguments.Add("SignatureVersion", "2");
            newArguments.Add("SignatureMethod","HmacSHA256");
            newArguments.Add("Timestamp", DateTime.UtcNow.ToString("s"));
            newArguments.Add("Version", "2009-04-15");
            return newArguments;
        }

        private string GetRequestSignature(NameValueCollection arguments)
        {
            string requestDescription = GetRequestDescription(arguments);
            string signature = GetRequestSignature(requestDescription);
            return "&Signature=" + Uri.EscapeDataString(signature);
        }

        private string WriteQueryString(NameValueCollection arguments)
        {
            string argumentString = string.Join("&", arguments.AllKeys.Select(k => k + "=" + Uri.EscapeDataString(arguments[k])));
            argumentString += GetRequestSignature(arguments);
            return argumentString;
        }

        private string GetRequestDescription(NameValueCollection arguments)
        {
            return string.Format("{0}\n{1}\n/\n{2}",
                "GET",
                simpleDbUrl,
                string.Join("&",
                    arguments.AllKeys.OrderBy(k => k, new NaturalByteComparer())
                    .Select(k => string.Format("{0}={1}",
                        Uri.EscapeDataString(k),
                        Uri.EscapeDataString(arguments[k])))));
        }

        private string GetRequestSignature(string requestDescription)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey)))
            {
                string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestDescription)));
                return signature;
            }
        }

        private class NaturalByteComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.CompareOrdinal(x, y);
            }
        }
            
        private const string simpleDbUrl = "sdb.amazonaws.com";
        private const string simpleDbProtocol = "https";
    }
}

