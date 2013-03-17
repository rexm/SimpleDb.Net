using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Linq;
using Cucumber.SimpleDb.ServiceBus;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Session
{
    internal class ProxySimpleDbDomain : ISimpleDbDomain
    {
        private string _name;
        private Dictionary<string, ISimpleDbDomain> _loadedDomains;
        private SessionSimpleDbDomain _realDomain;
        private readonly IInternalContext _context;

        internal ProxySimpleDbDomain(string name, Dictionary<string, ISimpleDbDomain> loadedDomains, IInternalContext context)
        {
            _name = name;
            _loadedDomains = loadedDomains;
            _context = context;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ISimpleDbItemCollection Items
        {
            get
            {
                if (_realDomain == null)
                {
                    return new SessionSimpleDbItemCollection(_context, this, new SimpleDbQueryProvider(_context));
                }
                else
                {
                    return _realDomain.Items;
                }
            }
        }

        public long AttributeNameCount
        {
            get { return this.RealDomain.AttributeNameCount; }
        }

        public long AttributeValueCount
        {
            get { return this.RealDomain.AttributeValueCount; }
        }

        public long TotalItemNameSize
        {
            get { return this.RealDomain.TotalItemNameSize; }
        }

        public long TotalAttributeValueSize
        {
            get { return this.RealDomain.TotalAttributeValueSize; }
        }

        public long TotalAttributeNameSize
        {
            get { return this.RealDomain.TotalAttributeNameSize; }
        }

        public void Delete()
        {
            _context.Service.DeleteDomain(_name); //TODO: switch to deferred/command pattern
            _loadedDomains.Remove(_name);
            _realDomain = null;
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

        private void LoadRealDomain()
        {
            XElement data = _context.Service.GetDomainMeta(_name);
            _realDomain = new SessionSimpleDbDomain(_context, _name, data);
        }
    }
}
