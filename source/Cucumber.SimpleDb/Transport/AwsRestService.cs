using System;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Xml;
using System.Net;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Utilities;
using System.IO;

namespace Cucumber.SimpleDb.Transport
{
    internal class AwsRestService : IAwsRestService
    {
        private readonly string _publicKey;
        private readonly string _privateKey;
        private readonly IWebRequestProvider _webRequest;
        private readonly string _simpleDbUrl;
        private static readonly XNamespace sdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";

        public AwsRestService(string publicKey, string privateKey, IWebRequestProvider webRequest)
            : this(publicKey, privateKey, "sdb.amazonaws.com",webRequest)
        {
        }

        public AwsRestService(string publicKey, string privateKey,string simpleDbUrl, IWebRequestProvider webRequest)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
            _simpleDbUrl = simpleDbUrl;
            _webRequest = webRequest;
        }

        public XElement ExecuteRequest(NameValueCollection arguments)
        {
            arguments = AddStandardArguments(arguments);
            string argumentString = WriteQueryString(arguments);
            var request = _webRequest.Create(string.Format(
                "{0}://{1}/?{2}",
                simpleDbProtocol,
                _simpleDbUrl,
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
                    string.Join("\n", errors.Descendants(sdbNs + "Error")
                        .Select(error => string.Format("{0}: {1}",
                            error.Element(sdbNs + "Code").Value,
                            error.Element(sdbNs + "Message").Value)))),
                    ex);
            }
        }

        private static XElement ProcessAwsResponse (WebResponse response)
        {
            using (var stream = response.GetResponseStream ())
            {
                try
                {
                    return XDocument.Load(stream).Root;
                }
                catch (XmlException xmlex)
                {
                    using (var reader = new StreamReader (stream, Encoding.UTF8))
                    {
                        var content = reader.ReadToEnd ();
                        throw new SimpleDbException (string.Format(
                                "AWS returned invalid XML:\n{0}", content)
                            , xmlex);
                    }
                }
                catch (InvalidOperationException invalidex)
                {
                    using (var reader = new StreamReader (stream, Encoding.UTF8))
                    {
                        var content = reader.ReadToEnd ();
                        throw new SimpleDbException (string.Format(
                            "AWS returned invalid XML:\n{0}", content)
                            , invalidex);
                    }
                }
            }
        }

        private NameValueCollection AddStandardArguments(NameValueCollection arguments)
        {
            var newArguments = new NameValueCollection(arguments);
            newArguments.Add("AWSAccessKeyId", _publicKey);
            newArguments.Add("SignatureVersion", "2");
            newArguments.Add("SignatureMethod","HmacSHA256");
            newArguments.Add("Timestamp", DateTime.UtcNow.ToString("o"));
            newArguments.Add("Version", "2009-04-15");
            return newArguments;
        }

        private string WriteQueryString(NameValueCollection arguments)
        {
            string argumentString = string.Join("&", arguments.AllKeys
                .Select(k => k + "=" + arguments[k].ToRfc3986()));
            argumentString += GetRequestSignature(arguments);
            return argumentString;
        }

        private string GetRequestSignature(NameValueCollection arguments)
        {
            string requestDescription = GetRequestDescription(arguments);
            string signature = HashString(requestDescription);
            return "&Signature=" + signature.ToRfc3986();
        }

        private string GetRequestDescription(NameValueCollection arguments)
        {
            return string.Format("{0}\n{1}\n/\n{2}",
                "GET",
                _simpleDbUrl,
                string.Join("&",
                    arguments.AllKeys.OrderBy(k => k, new NaturalByteComparer())
                    .Select(k => string.Format("{0}={1}",
                        k.ToRfc3986(),
                        arguments[k].ToRfc3986()))));
        }

        private string HashString(string requestDescription)
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
            
        private const string simpleDbProtocol = "https";
    }
}

