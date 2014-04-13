using System.Collections.Generic;

namespace Cucumber.SimpleDb.Async.Session
{
    internal class ProxySimpleDbDomain : ISimpleDbDomain
    {
        private readonly IInternalContext _context;
        private readonly Dictionary<string, ISimpleDbDomain> _loadedDomains;
        private readonly string _name;
        private SessionSimpleDbDomain _realDomain;

        internal ProxySimpleDbDomain(string name, Dictionary<string, ISimpleDbDomain> loadedDomains, IInternalContext context)
        {
            _name = name;
            _loadedDomains = loadedDomains;
            _context = context;
        }

        private ISimpleDbDomain RealDomain
        {
            get
            {
                if (_realDomain == null)
                {
                    LoadRealDomain();
                }
                return _realDomain;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public ISimpleDbItemCollection Items
        {
            get
            {
                return _realDomain == null ? new SessionSimpleDbItemCollection(_context, this, new SimpleDbQueryProvider(_context)) : _realDomain.Items;
            }
        }

        public long AttributeNameCount
        {
            get { return RealDomain.AttributeNameCount; }
        }

        public long AttributeValueCount
        {
            get { return RealDomain.AttributeValueCount; }
        }

        public long TotalItemNameSize
        {
            get { return RealDomain.TotalItemNameSize; }
        }

        public long TotalAttributeValueSize
        {
            get { return RealDomain.TotalAttributeValueSize; }
        }

        public long TotalAttributeNameSize
        {
            get { return RealDomain.TotalAttributeNameSize; }
        }

        public void Delete()
        {
            _context.Service.DeleteDomain(_name); //TODO: switch to deferred/command pattern
            if (_loadedDomains != null)
            {
                _loadedDomains.Remove(_name);
            }
            _realDomain = null;
        }

        private void LoadRealDomain()
        {
            var data = _context.Service.GetDomainMeta(_name);
            _realDomain = new SessionSimpleDbDomain(_context, _name, data);
        }
    }
}