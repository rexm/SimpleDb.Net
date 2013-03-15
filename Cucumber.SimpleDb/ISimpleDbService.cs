using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cucumber.SimpleDb
{
    public interface ISimpleDbService
    {
        XElement BatchDeleteAttributes(string domain, params object[] items);
        XElement BatchPutAttributes(string domain, params object[] items);
        XElement CreateDomain(string domain);
        XElement DeleteDomain(string domain);
        XElement DeleteAttributes(string domain, string itemName, params object[] attributes);
        XElement GetDomainMeta(string domain);
        XElement ListDomains();
        XElement ListDomains(string nextPageToken);
        XElement PutAttributes(string domain, string name, params object[] attributes);
        XElement GetAttributes(string domain, string name, bool useConsistency, params string[] attributeNames);
        XElement Select(string query, bool useConsistency);
        XElement Select(string query, bool useConsistency, string nextPageToken);
    }
}
