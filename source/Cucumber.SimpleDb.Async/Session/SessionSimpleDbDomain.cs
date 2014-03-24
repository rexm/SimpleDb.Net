using System;
using System.Xml.Linq;
using Cucumber.SimpleDb.Async.Linq;

namespace Cucumber.SimpleDb.Async.Session
{
    internal class SessionSimpleDbDomain : ISimpleDbDomain
    {
        private static readonly XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly long _attributeNameCount;
        private readonly long _attributeValueCount;
        private readonly IInternalContext _context;
        private readonly string _name;
        private readonly long _totalAttributeNameSize;
        private readonly long _totalAttributeValueSize;
        private readonly long _totalItemNameSize;

        internal SessionSimpleDbDomain(IInternalContext context, string name, XContainer data)
        {
            _name = name;
            _context = context;
            try
            {
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

        public ISimpleDbItemCollection Items
        {
            get { return new SessionSimpleDbItemCollection(_context, this, new SimpleDbQueryProvider(_context)); }
        }

        public string Name
        {
            get { return _name; }
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

        public void Delete()
        {
            _context.Service.DeleteDomain(_name); //TODO: switch to deferred/command pattern
        }
    }
}