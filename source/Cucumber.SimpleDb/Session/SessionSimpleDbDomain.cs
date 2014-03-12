using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;
using Cucumber.SimpleDb.Linq;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionSimpleDbDomain : ISimpleDbDomain
    {
        private readonly IInternalContext _context;
        private readonly XElement _data;
        private readonly string _name;
        private readonly long _attributeNameCount;
        private readonly long _attributeValueCount;
        private readonly long _totalItemNameSize;
        private readonly long _totalAttributeValueSize;
        private readonly long _totalAttributeNameSize;
        private readonly long _itemCount;

        internal SessionSimpleDbDomain(IInternalContext context, string name, XElement data)
        {
            _name = name;
            _data = data;
            _context = context;
            try
            {
                _attributeNameCount = long.Parse(_data.Element("AttributeNameCount").Value);
                _attributeValueCount = long.Parse(_data.Element("AttributeValueCount").Value);
                _totalItemNameSize = long.Parse(_data.Element("TotalItemNameSize").Value);
                _totalAttributeValueSize = long.Parse(_data.Element("TotalAttributeValueSize").Value);
                _totalAttributeNameSize = long.Parse(_data.Element("TotalAttributeNameSize").Value);
                _itemCount = long.Parse(_data.Element("ItemCount").Value);
            }
            catch (Exception ex)
            {
                throw new SimpleDbException("The response from SimpleDB was not valid", ex);
            }
        }

        public ISimpleDbItemCollection Items
        {
            get
            {
                return new SessionSimpleDbItemCollection(_context, this, new SimpleDbQueryProvider(_context));
            }
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
            _context.Service.DeleteDomain(this._name); //TODO: switch to deferred/command pattern
        }
    }
}
