using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cucumber.SimpleDb.Async.Linq;

namespace Cucumber.SimpleDb.Async.Session
{
    internal sealed class RealSimpleDbDomain : ISimpleDbDomain
    {
        private static readonly XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly long _attributeNameCount;
        private readonly long _attributeValueCount;
        private readonly IInternalContext _context;
        private readonly string _name;
        private readonly long _totalAttributeNameSize;
        private readonly long _totalAttributeValueSize;
        private readonly long _totalItemNameSize;

        internal RealSimpleDbDomain(IInternalContext context, string name, XContainer data)
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

        public Task<long> GetAttributeNameCountAsync()
        {
            return Task.FromResult(_attributeNameCount);
        }

        public Task<long> GetAttributeValueCountAsync()
        {
            return Task.FromResult(_attributeValueCount);
        }

        public Task<long> GetTotalItemNameSizeAsync()
        {
            return Task.FromResult(_totalItemNameSize);
        }

        public Task<long> GetTotalAttributeValueSizeAsync()
        {
            return Task.FromResult(_totalAttributeValueSize);
        }

        public Task<long> GetTotalAttributeNameSizeAsync()
        {
            return Task.FromResult(_totalAttributeNameSize);
        }

        public async Task DeleteAsync()
        {
            await _context.Service.DeleteDomainAsync(_name).ConfigureAwait(false); //TODO: switch to deferred/command pattern
        }
    }
}