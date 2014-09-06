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
        private readonly long _totalItemCount;
        private readonly ISimpleDbDomain _domain;

        internal SimpleDbDomainMetadata(ISimpleDbDomain domain, XContainer data)
        {
            _domain = domain;
            try
            {
                _totalItemCount = long.Parse(data.Element(SdbNs + "TotalItemCount").Value);
                _attributeNameCount = long.Parse(data.Element(SdbNs + "AttributeNameCount").Value);
                _attributeValueCount = long.Parse(data.Element(SdbNs + "AttributeValueCount").Value);
                _totalItemNameSize = long.Parse(data.Element(SdbNs + "TotalItemNameSize").Value);
                _totalAttributeValueSize = long.Parse(data.Element(SdbNs + "TotalAttributeValueSize").Value);
                _totalAttributeNameSize = long.Parse(data.Element(SdbNs + "TotalAttributeNameSize").Value);
            }
            catch (Exception ex)
            {
                throw new SimpleDbException("The response from SimpleDB was not valid", ex);
            }
        }

        public long TotalItemCount
        {
            get { return _totalItemCount; }
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
    }
}