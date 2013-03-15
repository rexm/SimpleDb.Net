using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Net;

namespace Cucumber.SimpleDb.ServiceBus
{
    internal class SimpleDbSoapService : ISimpleDbService
    {
        private readonly string _publicKey;
        private readonly string _privateKey;

        public SimpleDbSoapService(string publicKey, string privateKey)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        public XElement BatchDeleteAttributes(string domain, params object[] items)
        {
            using (var request = CreateRequest("BatchDeleteAttributes"))
            {
                try
                {
                    var writer = request.GetWriter();
                    writer.WriteStartElement("DomainName");
                    writer.WriteValue(domain);
                    writer.WriteEndElement();
                    int itemCount = 0;
                    foreach (dynamic item in items)
                    {
                        if (itemCount >= 25)
                        {
                            throw new SimpleDbException("Batch put is limited to 25 items per request");
                        }
                        writer.WriteStartElement("Item");
                        if (item.Reflect().HasMember().Attributes)
                        {
                            foreach (dynamic attribute in item.Attribtues)
                            {
                                writer.WriteStartElement("Attribute");
                                writer.WriteStartElement("Name");
                                writer.WriteValue(attribute.Name);
                                writer.WriteEndElement();
                                if (attribute.Reflect().HasMember().Value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteValue(attribute.Value);
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }
                        }
                        writer.WriteEndElement();
                        itemCount++;
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("One or more item definitions did not contain the expected properties", ex);
                }
                return request.GetResponse();
            }
        }

        public XElement BatchPutAttributes(string domain, params object[] items)
        {
            using (var request = CreateRequest("BatchPutAttributes"))
            {
                try
                {
                    var writer = request.GetWriter();
                    writer.WriteStartElement("DomainName");
                    writer.WriteValue(domain);
                    writer.WriteEndElement();
                    int itemCount = 0;
                    foreach (dynamic item in items)
                    {
                        if (itemCount >= 25)
                        {
                            throw new SimpleDbException("Batch put is limited to 25 items per request");
                        }
                        writer.WriteStartElement("Item");
                        if (item.Reflect().HasMember().Attributes)
                        {
                            foreach (dynamic attribute in item.Attribtues)
                            {
                                writer.WriteStartElement("Attribute");
                                writer.WriteStartElement("Name");
                                writer.WriteValue(attribute.Name);
                                writer.WriteEndElement();
                                writer.WriteStartElement("Value");
                                writer.WriteValue(attribute.Value);
                                writer.WriteEndElement();
                                if (attribute.Reflect().HasMember().Replace && attribute.Replace == true)
                                {
                                    writer.WriteStartElement("Replace");
                                    writer.WriteValue(true);
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }
                        }
                        writer.WriteEndElement();
                        itemCount++;
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("One or more item definitions did not contain the expected properties", ex);
                }
                return request.GetResponse();
            }
        }

        public XElement CreateDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domain");
            }
            using (var request = CreateRequest("CreateDomain"))
            {
                var writer = request.GetWriter();
                writer.WriteStartElement("DomainName");
                writer.WriteValue(domain);
                writer.WriteEndElement();
                return request.GetResponse();
            }
        }

        public XElement DeleteDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domain");
            }
            using (var request = CreateRequest("CreateDomain"))
            {
                var writer = request.GetWriter();
                writer.WriteStartElement("DomainName");
                writer.WriteValue(domain);
                writer.WriteEndElement();
                return request.GetResponse();
            }
        }

        public XElement DeleteAttributes(string domain, string itemName, params object[] attributes)
        {
            using (var request = CreateRequest("DeleteAttributes"))
            {
                try
                {
                    var writer = request.GetWriter();
                    writer.WriteStartElement("DomainName");
                    writer.WriteValue(domain);
                    writer.WriteEndElement();
                    writer.WriteStartElement("ItemName");
                    writer.WriteValue(itemName);
                    writer.WriteEndElement();
                    foreach (dynamic attribute in attributes)
                    {
                        writer.WriteStartElement("Attribute");
                        writer.WriteStartElement("Name");
                        writer.WriteValue(attribute.Name);
                        writer.WriteEndElement();
                        if (attribute.Reflect().HasMember().Value)
                        {
                            writer.WriteStartElement("Value");
                            writer.WriteValue(attribute.Value);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("One or more item definitions did not contain the expected properties", ex);
                }
                return request.GetResponse();
            }
        }

        public XElement GetDomainMeta(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domain");
            }
            using (var request = CreateRequest("DomainMetadata"))
            {
                var writer = request.GetWriter();
                writer.WriteStartElement("DomainName");
                writer.WriteValue(domain);
                writer.WriteEndElement();
                return request.GetResponse();
            }
        }

        public XElement ListDomains()
        {
            using (var request = CreateRequest("ListDomains"))
            {
                return request.GetResponse();
            }
        }

        public XElement ListDomains(string nextPageToken)
        {
            if (string.IsNullOrEmpty(nextPageToken))
            {
                throw new ArgumentNullException("nextPageToken");
            }
            using (var request = CreateRequest("ListDomains"))
            {
                var writer = request.GetWriter();
                writer.WriteStartElement("NextToken");
                writer.WriteValue(nextPageToken);
                writer.WriteEndElement();
                return request.GetResponse();
            }
        }

        public XElement PutAttributes(string domain, string itemName, params object[] attributes)
        {
            using (var request = CreateRequest("PutAttributes"))
            {
                try
                {
                    var writer = request.GetWriter();
                    writer.WriteStartElement("DomainName");
                    writer.WriteValue(domain);
                    writer.WriteEndElement();
                    writer.WriteStartElement("ItemName");
                    writer.WriteValue(itemName);
                    writer.WriteEndElement();
                    foreach (dynamic attribute in attributes)
                    {
                        writer.WriteStartElement("Attribute");
                        writer.WriteStartElement("Name");
                        writer.WriteValue(attribute.Name);
                        writer.WriteEndElement();
                        writer.WriteStartElement("Value");
                        writer.WriteValue(attribute.Value);
                        writer.WriteEndElement();
                        if (attribute.Reflect().HasMember().Replace && attribute.Replace == true)
                        {
                            writer.WriteStartElement("Replace");
                            writer.WriteValue(true);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("One or more item definitions did not contain the expected properties", ex);
                }
                return request.GetResponse();
            }
        }

        public XElement GetAttributes(string domain, string itemName, bool useConsistency, params string[] attributeNames)
        {
            using (var request = CreateRequest("PutAttributes"))
            {
                try
                {
                    var writer = request.GetWriter();
                    writer.WriteStartElement("DomainName");
                    writer.WriteValue(domain);
                    writer.WriteEndElement();
                    writer.WriteStartElement("ItemName");
                    writer.WriteValue(itemName);
                    writer.WriteEndElement();
                    if (useConsistency)
                    {
                        writer.WriteStartElement("ConsistentRead");
                        writer.WriteValue(true);
                        writer.WriteEndElement();
                    }
                    foreach (string attributeName in attributeNames)
                    {
                        writer.WriteStartElement("AttributeName");
                        writer.WriteValue(attributeName);
                        writer.WriteEndElement();
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("One or more item definitions did not contain the expected properties", ex);
                }
                return request.GetResponse();
            }
        }

        public XElement Select(string query, bool useConsistency)
        {
            return Select(query, useConsistency, null);
        }

        public XElement Select(string query, bool useConsistency, string nextPageToken)
        {
            using (var request = CreateRequest("PutAttributes"))
            {
                try
                {
                    var writer = request.GetWriter();
                    writer.WriteStartElement("SelectExpression");
                    writer.WriteValue(query);
                    writer.WriteEndElement();
                    if (useConsistency)
                    {
                        writer.WriteStartElement("ConsistentRead");
                        writer.WriteValue(true);
                        writer.WriteEndElement();
                    }
                    if (!string.IsNullOrEmpty(nextPageToken))
                    {
                        writer.WriteStartElement("NextToken");
                        writer.WriteValue(nextPageToken);
                        writer.WriteEndElement();
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("One or more item definitions did not contain the expected properties", ex);
                }
                return request.GetResponse();
            }
        }

        private AmazonSoapRequest CreateRequest(string action)
        {
            return new AmazonSoapRequest(action, _publicKey, _privateKey);
        }

        private class AmazonSoapRequest : IDisposable
        {
            private readonly string _publicKey;
            private readonly string _privateKey;
            private readonly string _action;
            private readonly WebRequest _request;
            private readonly AmazonSoapWriter _writer;

            public AmazonSoapRequest(string action, string publicKey, string privateKey)
            {
                _action = action;
                _publicKey = publicKey;
                _privateKey = privateKey;
                _request = HttpWebRequest.Create(string.Format("{0}://{1}",
                    simpleDbProtocol,
                    simpleDbUrl));
                _request.Method = "POST";
                _writer = new AmazonSoapWriter(
                    action,
                    _publicKey,
                    _privateKey,
                    _request.GetRequestStream());
            }

            public XmlWriter GetWriter()
            {
                return _writer;
            }

            public XElement GetResponse()
            {

                try
                {
                    using (var response = _request.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            return XDocument.Load(stream).Root;
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

            public void Dispose()
            {
                _writer.Close();
            }

            private const string simpleDbUrl = "sdb.amazonaws.com";
            private const string simpleDbProtocol = "https";

            private class AmazonSoapWriter : XmlTextWriter
            {
                private readonly string _publicKey;
                private readonly string _privateKey;
                private readonly string _action;
                private readonly string _timeStamp;

                public AmazonSoapWriter(string action, string publicKey, string privateKey, Stream targetStream)
                    : base(targetStream, Encoding.UTF8)
                {
                    _action = action;
                    _timeStamp = DateTime.Now.ToString("s");
                    _publicKey = publicKey;
                    _privateKey = privateKey;
                    this.Namespaces = true;
                    OpenEnvelope();
                }

                public override void Close()
                {
                    CloseEnvelope();
                    base.Close();
                }

                private void OpenEnvelope()
                {
                    this.WriteStartDocument();
                    this.WriteStartElement("Envelope", "soapenv");
                    this.WriteStartAttribute("soapenv", "xmlns");
                    this.WriteValue("http://schemas.xmlsoap.org/soap/envelope/");
                    this.WriteEndAttribute();
                    this.WriteStartAttribute("xsd", "xmlns");
                    this.WriteValue("http://www.w3.org/2001/XMLSchema");
                    this.WriteEndAttribute();
                    this.WriteStartAttribute("xsi", "xmlns");
                    this.WriteValue("http://www.w3.org/2001/XMLSchema-instance");
                    this.WriteEndAttribute();
                    this.WriteStartElement("Header", "soapenv");
                    this.WriteStartAttribute("aws", "xmlns");
                    this.WriteValue("http://security.amazonaws.com/doc/2009-4-15/");
                    this.WriteEndAttribute();
                    this.WriteStartElement("AWSAccessKeyId", "aws");
                    this.WriteValue(_publicKey);
                    this.WriteEndElement();
                    this.WriteStartElement("Timestamp", "aws");
                    this.WriteValue(_timeStamp);
                    this.WriteEndElement();
                    this.WriteStartElement("Signature", "aws");
                    this.WriteValue(GenerateSignature());
                    this.WriteEndElement();
                    this.WriteEndElement();
                    this.WriteStartElement("Body", "soapenv");
                    this.WriteStartElement(_action);
                    this.WriteStartAttribute("xmlns");
                    this.WriteValue("http://sdb.amazonaws.com/doc/2009-04-15");
                    this.WriteEndAttribute();
                }

                private void CloseEnvelope()
                {
                    this.WriteEndElement();
                    this.WriteEndElement();
                    this.WriteEndElement();
                    this.WriteEndDocument();
                }

                private string GenerateSignature()
                {
                    using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey)))
                    {
                        string message = string.Format("{0}{1}", _action, _timeStamp);
                        string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
                        return signature;
                    }
                }
            }
        }
    }
}
