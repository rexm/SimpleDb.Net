using System;
using System.Xml.Linq;
using Cucumber.SimpleDb.Linq;

namespace Cucumber.SimpleDb.Session
{
    internal class SimpleDbDomainMetadata : ISimpleDbDomainMetadata
    {
        private static readonly XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly long _attributeNameCount;
        private readonly long _attributeValueCount;
        private readonly long _totalAttributeNameSize;
        private readonly long _totalAttributeValueSize;
        private readonly long _totalItemNameSize;
        private readonly long _itemCount;
        private readonly DateTime _calculated;
        private readonly ISimpleDbDomain _domain;

        internal SimpleDbDomainMetadata(ISimpleDbDomain domain, XContainer data)
        {
            _domain = domain;
            try
            {
                _itemCount = long.Parse(data.Element(SdbNs + "ItemCount").Value);
                _attributeNameCount = long.Parse(data.Element(SdbNs + "AttributeNameCount").Value);
                _attributeValueCount = long.Parse(data.Element(SdbNs + "AttributeValueCount").Value);
                _totalItemNameSize = long.Parse(data.Element(SdbNs + "ItemNamesSizeBytes").Value);
                _totalAttributeValueSize = long.Parse(data.Element(SdbNs + "AttributeValuesSizeBytes").Value);
                _totalAttributeNameSize = long.Parse(data.Element(SdbNs + "AttributeNamesSizeBytes").Value);
                _calculated = GetDateTimeFromEpoch(long.Parse(data.Element(SdbNs + "Timestamp").Value));
            }
            catch (Exception ex)
            {
                throw new SimpleDbException("The response from SimpleDB was not valid", ex);
            }
        }

        public DateTime Calculated
        {
            get { return _calculated; }
        }

        public long ItemCount
        {
            get { return _itemCount; }
        }

        public long AttributeNameCount
        {
            get { return _attributeNameCount; }
        }

        public long AttributeValueCount
        {
            get { return _attributeValueCount; }
        }

        public long TotalItemNameSize
        {
            get { return _totalItemNameSize; }
        }

        public long TotalAttributeValueSize
        {
            get { return _totalAttributeValueSize; }
        }

        public long TotalAttributeNameSize
        {
            get { return _totalAttributeNameSize; }
        }

        private static DateTime GetDateTimeFromEpoch(long seconds)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0);
            return dt.AddSeconds(seconds);
        }
    }
}