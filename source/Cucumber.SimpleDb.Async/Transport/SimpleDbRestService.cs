using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cucumber.SimpleDb.Async.Utilities;

namespace Cucumber.SimpleDb.Async.Transport
{
    internal sealed class SimpleDbRestService : ISimpleDbService
    {
        private readonly IAwsRestService _restService;

        public SimpleDbRestService(IAwsRestService restService)
        {
            _restService = restService;
        }

        public Task<XElement> BatchPutAttributesAsync(string domain, params object[] items)
        {
            if (items.Length < 1)
            {
                throw new ArgumentOutOfRangeException("items", "Must put at least 1 item");
            }
            var values = new NameValueCollection
            {
                {
                    "Action", "BatchPutAttributes"
                },
                {
                    "DomainName", domain
                }
            };
            var itemCount = 0;
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
                    if (ObjectExtensions.HasMember(item, "Attributes"))
                    {
                        var attributeCount = 0;
                        foreach (var attribute in item.Attributes)
                        {
                            values.Add(
                                string.Format("Item.{0}.Attribute.{1}.Name", itemCount, attributeCount),
                                attribute.Name);
                            values.Add(
                                string.Format("Item.{0}.Attribute.{1}.Value", itemCount, attributeCount),
                                attribute.Value.ToString());
                            if (ObjectExtensions.HasMember(attribute, "Replace"))
                            {
                                if (attribute.Replace == true)
                                {
                                    values.Add(
                                        string.Format("Item.{0}.Attribute.{1}.Replace", itemCount, attributeCount),
                                        "true");
                                }
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
            return InternalExecuteAsync(values);
        }

        public Task<XElement> CreateDomainAsync(string domain)
        {
            var values = new NameValueCollection
            {
                {
                    "Action", "CreateDomain"
                },
                {
                    "DomainName", domain
                }
            };
            return InternalExecuteAsync(values);
        }

        public Task<XElement> DeleteDomainAsync(string domain)
        {
            var values = new NameValueCollection
            {
                {
                    "Action", "DeleteDomain"
                },
                {
                    "DomainName", domain
                }
            };
            return InternalExecuteAsync(values);
        }

        public Task<XElement> GetDomainMetaAsync(string domain)
        {
            var values = new NameValueCollection
            {
                {
                    "Action", "DomainMetadata"
                },
                {
                    "DomainName", domain
                }
            };
            return InternalExecuteAsync(values);
        }

        public Task<XElement> ListDomainsAsync()
        {
            return ListDomainsAsync(null);
        }

        public Task<XElement> ListDomainsAsync(string nextPageToken)
        {
            var values = new NameValueCollection
            {
                {
                    "Action", "ListDomains"
                }
            };
            if (!string.IsNullOrEmpty(nextPageToken))
            {
                values.Add("NextToken", nextPageToken);
            }
            return InternalExecuteAsync(values);
        }

        public Task<XElement> PutAttributesAsync(string domain, string itemName, params object[] attributes)
        {
            var values = new NameValueCollection
            {
                {
                    "Action", "PutAttributes"
                },
                {
                    "DomainName", domain
                },
                {
                    "ItemName", itemName
                }
            };
            var attributeCount = 0;
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
                    if (ObjectExtensions.HasMember(attribute, "Replace") && attribute.Replace == true)
                    {
                        values.Add(
                            string.Format("Attribute.{0}.Replace", attributeCount),
                            "true");
                    }
                    if (ObjectExtensions.HasMember(attribute, "When"))
                    {
                        var when = attribute.When;
                        values.Add(
                            string.Format("Expected.{0}.Name", attributeCount),
                            attribute.Name);
                        if (ObjectExtensions.HasMember(when, "Value"))
                        {
                            values.Add(
                                string.Format("Expected.{0}.Value", attributeCount),
                                when.Value);
                        }
                        if (ObjectExtensions.HasMember(when, "Exists"))
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
            return InternalExecuteAsync(values);
        }

        public Task<XElement> GetAttributesAsync(string domain, string itemName, bool useConsistency, params string[] attributeNames)
        {
            var values = new NameValueCollection
            {
                {
                    "Action", "GetAttributes"
                },
                {
                    "DomainName", domain
                },
                {
                    "ItemName", itemName
                }
            };
            if (useConsistency)
            {
                values.Add("ConsistentRead", useConsistency.ToString());
            }
            var attributeCount = 0;
            foreach (var attributeName in attributeNames)
            {
                values.Add(
                    string.Format("Attribute.{0}", attributeCount),
                    attributeName);
                attributeCount++;
            }
            return InternalExecuteAsync(values);
        }

        public Task<XElement> DeleteAttributesAsync(string domain, string itemName, params object[] attributes)
        {
            var values = new NameValueCollection
            {
                {
                    "Action", "DeleteAttributes"
                },
                {
                    "DomainName", domain
                },
                {
                    "ItemName", itemName
                }
            };
            var attributeCount = 0;
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
                    if (ObjectExtensions.HasMember(attribute, "When"))
                    {
                        var when = attribute.When;
                        values.Add(
                            string.Format("Expected.{0}.Name", attributeCount),
                            attribute.Name);
                        if (ObjectExtensions.HasMember(when, "Value"))
                        {
                            values.Add(
                                string.Format("Expected.{0}.Value", attributeCount),
                                when.Value);
                        }
                        if (ObjectExtensions.HasMember(when, "Exists"))
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
            return InternalExecuteAsync(values);
        }

        public Task<XElement> BatchDeleteAttributesAsync(string domain, params object[] items)
        {
            if (items.Length < 1)
            {
                throw new ArgumentOutOfRangeException("items", "Must delete at least 1 item");
            }
            var values = new NameValueCollection
            {
                {
                    "Action", "BatchDeleteAttributes"
                },
                {
                    "DomainName", domain
                },
            };
            var itemCount = 0;
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
                    if (ObjectExtensions.HasMember(item, "Attributes"))
                    {
                        var attributeCount = 0;
                        foreach (var attribute in item.Attributes)
                        {
                            values.Add(
                                string.Format("Item.{0}.Attribute.{1}.Name", itemCount, attributeCount),
                                attribute.Name);
                            values.Add(
                                string.Format("Item.{0}.Attribute.{1}.Value", itemCount, attributeCount),
                                attribute.Value.ToString());
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
            return InternalExecuteAsync(values);
        }

        public Task<XElement> SelectAsync(string query, bool useConsistency)
        {
            return SelectAsync(query, useConsistency, null);
        }

        public Task<XElement> SelectAsync(string query, bool useConsistency, string nextPageToken)
        {
            var values = new NameValueCollection
            {
                {
                    "Action", "Select"
                },
                {
                    "SelectExpression", query
                },
            };
            if (useConsistency)
            {
                values.Add("ConsistentRead", useConsistency.ToString());
            }
            if (!string.IsNullOrEmpty(nextPageToken))
            {
                values.Add("NextToken", nextPageToken);
            }
            return InternalExecuteAsync(values);
        }

        private Task<XElement> InternalExecuteAsync(NameValueCollection arguments)
        {
            return _restService.ExecuteRequestAsync(arguments);
        }
    }
}