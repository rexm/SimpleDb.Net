using System.Collections.Generic;
using Cucumber.SimpleDb.Linq;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionSimpleDbDomain : ISimpleDbDomain
    {
        private readonly IInternalContext _context;
        private readonly Dictionary<string, ISimpleDbDomain> _loadedDomains;
        private readonly string _name;
        private readonly SessionSimpleDbItemCollection _items;

        internal SessionSimpleDbDomain(
            string name,
            Dictionary<string, ISimpleDbDomain> loadedDomains,
            IInternalContext context)
        {
            _name = name;
            _loadedDomains = loadedDomains;
            _context = context;
            _items = new SessionSimpleDbItemCollection(_context, this, new SimpleDbQueryProvider(_context));
        }

        public string Name
        {
            get { return _name; }
        }

        public ISimpleDbItemCollection Items
        {
            get { return _items; }
        }

        public void Delete()
        {
            DeleteAsync().Wait();
        }

        public async Task DeleteAsync()
        {
            await _context.Service.DeleteDomainAsync(_name);
            if (_loadedDomains != null)
            {
                _loadedDomains.Remove(_name);
            }
        }

        public async Task<ISimpleDbDomainMetadata> GetDomainInfoAsync()
        {
            var data = await _context.Service.GetDomainMetaAsync(_name);
            return new SimpleDbDomainMetadata(this, data);
        }
    }
}