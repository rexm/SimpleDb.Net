﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Async.Transport
{
    internal class AwsRestService : IAwsRestService
    {
        private const string SimpleDbUrl = "sdb.amazonaws.com";
        private const string SimpleDbProtocol = "https";
        private static readonly XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly string _privateKey;
        private readonly string _publicKey;
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
            var argumentString = WriteQueryString(arguments);
            var request = _webRequest.Create(string.Format(
                "{0}://{1}/?{2}",
                SimpleDbProtocol,
                SimpleDbUrl,
                argumentString));
            try
            {
                using (var response = request.GetResponse())
                {
                    return ProcessAwsResponse(response);
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                var errors = ProcessAwsResponse(response);
                throw new SimpleDbException(string.Format(
                    "Error {0} {1}: AWS returned the following:\n{2}",
                    (int) response.StatusCode,
                    response.StatusCode,
                    string.Join("\n", errors.Descendants(SdbNs + "Error")
                        .Select(error => string.Format("{0}: {1}",
                            error.Element(SdbNs + "Code").Value,
                            error.Element(SdbNs + "Message").Value)))),
                    ex);
            }
        }

        private static XElement ProcessAwsResponse(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                try
                {
                    return XDocument.Load(stream).Root;
                }
                catch (XmlException xmlex)
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var content = reader.ReadToEnd();
                        throw new SimpleDbException(string.Format(
                            "AWS returned invalid XML:\n{0}", content)
                            , xmlex);
                    }
                }
                catch (InvalidOperationException invalidex)
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var content = reader.ReadToEnd();
                        throw new SimpleDbException(string.Format(
                            "AWS returned invalid XML:\n{0}", content)
                            , invalidex);
                    }
                }
            }
        }

        private NameValueCollection AddStandardArguments(NameValueCollection arguments)
        {
            var newArguments = new NameValueCollection(arguments)
            {
                {
                    "AWSAccessKeyId", _publicKey
                },
                {
                    "SignatureVersion", "2"
                },
                {
                    "SignatureMethod", "HmacSHA256"
                },
                {
                    "Timestamp", DateTime.UtcNow.ToString("o")
                },
                {
                    "Version", "2009-04-15"
                }
            };
            return newArguments;
        }

        private string WriteQueryString(NameValueCollection arguments)
        {
            var argumentString = string.Join("&", arguments.AllKeys
                .Select(k => k + "=" + arguments[k].ToRfc3986()));
            argumentString += GetRequestSignature(arguments);
            return argumentString;
        }

        private string GetRequestSignature(NameValueCollection arguments)
        {
            var requestDescription = GetRequestDescription(arguments);
            var signature = HashString(requestDescription);
            return "&Signature=" + signature.ToRfc3986();
        }

        private static string GetRequestDescription(NameValueCollection arguments)
        {
            return string.Format("{0}\n{1}\n/\n{2}",
                "GET",
                SimpleDbUrl,
                string.Join("&",
                    arguments.AllKeys.OrderBy(k => k, new NaturalByteComparer())
                        .Select(k => string.Format("{0}={1}",
                            k.ToRfc3986(),
                            arguments[k].ToRfc3986()))));
        }

        private string HashString(string requestDescription)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey)))
            {
                var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestDescription)));
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
    }
}