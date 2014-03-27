using System;
using System.Net.Http;
using System.Threading.Tasks;
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
				private readonly HttpClient _httpClient;
        private static readonly XNamespace sdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";

				public AwsRestService(string publicKey, string privateKey)
					:this(publicKey, privateKey, new HttpClient())
				{
				
				}

        internal AwsRestService(string publicKey, string privateKey, HttpClient httpClient)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
            _httpClient = httpClient;
        }

				public XElement ExecuteRequest(NameValueCollection arguments)
				{
					return ExecuteRequestAsync(arguments).Result;
				}

        public Task<XElement> ExecuteRequestAsync(NameValueCollection arguments)
        {
            arguments = AddStandardArguments(arguments);
            var argumentString = WriteQueryString(arguments);
						var requestUrl = string.Format(
								"{0}://{1}/?{2}",
								simpleDbProtocol,
								simpleDbUrl,
								argumentString);
						var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUrl));
						return _httpClient.SendAsync(requestMessage).ContinueWith(task =>
						{
							var responseMessage = task.Result;

							if (responseMessage.IsSuccessStatusCode)
							{
								return ProcessAwsResponse(responseMessage);
							}

							var errors = ProcessAwsResponse(responseMessage);
							//TODO: (CV) Should not be throwing from the async handler. Maybe change the method signature to return a result wrapper which indicates the Status of result???.
							throw new SimpleDbException(string.Format(
									"Error {0} {1}: AWS returned the following:\n{2}",
									(int)responseMessage.StatusCode,
									responseMessage.StatusCode,
									string.Join("\n", errors.Descendants(sdbNs + "Error")
											.Select(error => string.Format("{0}: {1}",
													error.Element(sdbNs + "Code").Value,
													error.Element(sdbNs + "Message").Value)))));
						});

        }

        private static XElement ProcessAwsResponse (HttpResponseMessage response)
        {
						if (response.Content == null)
						{
							return null;
						}
            using (var stream = response.Content.ReadAsStreamAsync().Result)
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
                simpleDbUrl,
                string.Join("&",
                    arguments.AllKeys.OrderBy(k => k, new NaturalByteComparer())
                    .Select(k => string.Format("{0}={1}",
                        k.ToRfc3986(),
                        arguments[k].ToRfc3986()))));
        }

        internal string HashString(string requestDescription)
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

