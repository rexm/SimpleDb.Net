using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Xml;

namespace Cucumber.SimpleDb.ServiceBus
{
    internal class SimpleDbRestService : ISimpleDbService
    {
        private readonly string _publicKey;
        private readonly string _privateKey;

        public SimpleDbRestService(string publicKey, string privateKey)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        public XElement BatchPutAttributes(string domain, params object[] items)
        {
            var values = new NameValueCollection
            {
                {"DomainName", domain}
            };
            int itemCount = 0;
            try
            {
                foreach (dynamic item in items)
                {
                    if (itemCount >= 25)
                    {
                        throw new SimpleDbException("Batch put is limited to 25 items per request");
                    }
                    values.Add(
                        string.Format("Item.{0}.ItemName", itemCount),
                        item.Name);
                    if (item.Reflect().HasMember().Attributes)
                    {
                        int attributeCount = 0;
                        foreach (dynamic attribute in item.Attributes)
                        {
                            values.Add(
                                string.Format("Item.{0}.Attribute.{1}.Name", itemCount, attributeCount),
                                attribute.Name);
                            values.Add(
                                string.Format("Item.{0}.Attribute.{1}.Value", itemCount, attributeCount),
                                attribute.Value.ToString());
                            if (attribute.Reflect().HasMember().Replace && attribute.Replace == true)
                            {
                                values.Add(
                                    string.Format("Item.{0}.Attribute.{1}.Replace", itemCount, attributeCount),
                                    "true");
                            }
                            attributeCount++;
                        }
                    }
                    itemCount++;
                }
            }
            catch (Exception ex)
            {
                throw new FormatException("One or more item definitions did not contain the expected properties", ex);
            }
            return Fetch(values);
        }

        public XElement CreateDomain(string domain)
        {
            var values = new NameValueCollection
            {
                {"Action", "CreateDomain"},
                {"DomainName", domain}
            };
            return Fetch(values);
        }

        public XElement DeleteDomain(string domain)
        {
            var values = new NameValueCollection
            {
                {"Action", "DeleteDomain"},
                {"DomainName", domain}
            };
            return Fetch(values);
        }

        public XElement GetDomainMeta(string domain)
        {
            var values = new NameValueCollection
            {
                {"Action", "DomainMetadata"},
                {"DomainName", domain}
            };
            return Fetch(values);
        }

        public XElement ListDomains()
        {
            return ListDomains(null);
        }

        public XElement ListDomains(string nextPageToken)
        {
            var values = new NameValueCollection
            {
                {"Action", "ListDomains"}
            };
            if (!string.IsNullOrEmpty(nextPageToken))
            {
                values.Add("NextToken", nextPageToken);
            }
            return Fetch(values);
        }

        public XElement PutAttributes(string domain, string itemName, params object[] attributes)
        {
            var values = new NameValueCollection
            {
                {"Action", "PutAttributes"},
                {"DomainName", domain},
                {"ItemName", itemName}
            };
            int attributeCount = 0;
            try
            {
                foreach (dynamic attribute in attributes)
                {
                    values.Add(
                        string.Format("Attribute.{0}.Name", attributeCount),
                        attribute.Name);
                    values.Add(
                        string.Format("Attribute.{0}.Value", attributeCount),
                        attribute.Value);
                    if (attribute.Reflect().HasMember().Replace && attribute.Replace == true)
                    {
                        values.Add(
                            string.Format("Attribute.{0}.Replace", attributeCount),
                            "true");
                    }
                    if (attribute.Reflect().HasMember().When)
                    {
                        dynamic when = attribute.When;
                        values.Add(
                            string.Format("Expected.{0}.Name", attributeCount),
                            attribute.Name);
                        if (when.Reflect().HasMember().Value)
                        {
                            values.Add(
                                string.Format("Expected.{0}.Value", attributeCount),
                                when.Value);
                        }
                        if (when.Reflect().HasMember().Exists)
                        {
                            values.Add(
                                string.Format("Expected.{0}.Exists", attributeCount),
                                when.Exists.ToString());
                        }
                    }
                    attributeCount++;
                }
            }
            catch (Exception ex)
            {
                throw new FormatException("One or more item definitions did not contain the expected properties", ex);
            }
            return Fetch(values);
        }

        public XElement GetAttributes(string domain, string itemName, bool useConsistency, params string[] attributeNames)
        {
            var values = new NameValueCollection
            {
                {"Action", "GetAttributes"},
                {"DomainName", domain},
                {"ItemName", itemName}
            };
            if (useConsistency)
            {
                values.Add("ConsistentRead", useConsistency.ToString());
            }
            int attributeCount = 0;
            foreach (var attributeName in attributeNames)
            {
                values.Add(
                    string.Format("Attribute.{0}", attributeCount),
                    attributeName);
                attributeCount++;
            }
            return Fetch(values);
        }

        public XElement DeleteAttributes(string domain, string itemName, params object[] attributes)
        {
            var values = new NameValueCollection
            {
                {"Action","DeleteAttributes"},
                {"DomainName", domain},
                {"ItemName", itemName}
            };
            int attributeCount = 0;
            try
            {
                foreach (dynamic attribute in attributes)
                {
                    values.Add(
                        string.Format("Attribute.{0}.Name", attributeCount),
                        attribute.Name);
                    values.Add(
                        string.Format("Attribute.{0}.Value", attributeCount),
                        attribute.Value);
                    if (attribute.Reflect().HasMember().When)
                    {
                        dynamic when = attribute.When;
                        values.Add(
                            string.Format("Expected.{0}.Name", attributeCount),
                            attribute.Name);
                        if (when.Reflect().HasMember().Value)
                        {
                            values.Add(
                                string.Format("Expected.{0}.Value", attributeCount),
                                when.Value);
                        }
                        if (when.Reflect().HasMember().Exists)
                        {
                            values.Add(
                                string.Format("Expected.{0}.Exists", attributeCount),
                                when.Exists.ToString());
                        }
                    }
                    attributeCount++;
                }
            }
            catch (Exception ex)
            {
                throw new FormatException("One or more item definitions did not contain the expected properties", ex);
            }
            return Fetch(values);
        }

        public XElement BatchDeleteAttributes(string domain, params object[] items)
        {
            var values = new NameValueCollection
            {
                {"Action","BatchDeleteAttributes"},
                {"DomainName", domain},
            };
            int itemCount = 0;
            try
            {
                foreach (dynamic item in items)
                {
                    if (itemCount >= 25)
                    {
                        throw new SimpleDbException("Batch delete is limited to 25 items per request");
                    }
                    values.Add(
                        string.Format("Item.{0}.ItemName", itemCount),
                        item.Name);
                    if (item.Reflect().HasMember().Attributes)
                    {
                        int attributeCount = 0;
                        foreach (dynamic attribute in item.Attributes)
                        {
                            values.Add(
                                string.Format("Item.{0}.Attribute.{1}.Name", itemCount, attributeCount),
                                attribute.Name);
                            values.Add(
                                string.Format("Item.{0}.Attribute.{1}.Value", itemCount, attributeCount),
                                attribute.Value.ToString());
                            if (attribute.Reflect().HasMember().Replace && attribute.Replace == true)
                            {
                                values.Add(
                                    string.Format("Item.{0}.Attribute.{1}.Replace", itemCount, attributeCount),
                                    "true");
                            }
                            attributeCount++;
                        }
                    }
                    itemCount++;
                }
            }
            catch (Exception ex)
            {
                throw new FormatException("One or more item definitions did not contain the expected properties", ex);
            }
            return Fetch(values);
        }

        public XElement Select(string query, bool useConsistency)
        {
            return Select(query, useConsistency, null);
        }

        public XElement Select(string query, bool useConsistency, string nextPageToken)
        {
            var values = new NameValueCollection
            {
                {"Action", "Select"},
                {"SelectExpression", query},
            };
            if (useConsistency)
            {
                values.Add("ConsistentRead", useConsistency.ToString());
            }
            if (!string.IsNullOrEmpty(nextPageToken))
            {
                values.Add("NextToken", nextPageToken);
            }
            return Fetch(values);
        }

        private XElement Fetch(NameValueCollection arguments)
        {
            arguments = AddStandardArguments(arguments);
            string argumentString = WriteQueryString(arguments);
            var request = HttpWebRequest.Create(string.Format(
                "{0}://{1}/?{2}",
                simpleDbProtocol,
                simpleDbUrl,
                argumentString));
            try
            {
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    try
                    {
                        return XDocument.Load(stream).Root;
                    }
                    catch (XmlException xmlex)
                    {
                        throw new SimpleDbException("Amazon SimpleDb returned invalid XML", xmlex);
                    }
                    catch (InvalidOperationException invalidex)
                    {
                        throw new SimpleDbException("Amazon SimpleDb returned invalid XML", invalidex);
                    }
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                throw new SimpleDbException(string.Format(
                    "Amazon SimpleDb returned an error: {0} {1} / (\"{2}\")",
                    (int)response.StatusCode,
                    response.StatusCode,
                    response.StatusDescription),
                    ex);
            }
        }

        private NameValueCollection AddStandardArguments(NameValueCollection arguments)
        {
            var newArguments = new NameValueCollection(arguments);
            newArguments.Add("AWSAccessKeyId", _publicKey);
            newArguments.Add("SignatureVersion", "2");
            newArguments.Add("SignatureMethod","HmacSHA256");
            newArguments.Add("Timestamp", DateTime.Now.ToString("s"));
            newArguments.Add("Version", "2009-04-15");
            return newArguments;
        }

        private string WriteQueryString(NameValueCollection arguments)
        {
            string argumentString = string.Join("&", arguments.AllKeys.Select(k => k + "=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode(arguments[k]))));
            argumentString = AddRequestSignature(argumentString);
            return argumentString;
        }

        private string AddRequestSignature(string argumentString)
        {
            string signature = GetRequestSignature(argumentString);
            return argumentString += "&Signature=" + HttpUtility.UrlEncode(signature);
        }

        private string GetRequestSignature(string arguments)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey)))
            {
                string message = string.Format("GET\n{0}\n/\n{1}",
                    simpleDbUrl,
                    arguments);
                string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
                return signature;
            }
        }

        private const string simpleDbUrl = "sdb.amazonaws.com";
        private const string simpleDbProtocol = "https";
    }
}
