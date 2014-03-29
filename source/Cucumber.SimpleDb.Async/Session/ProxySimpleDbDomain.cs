using System.Collections.Generic;
using System.Threading.Tasks;
using Cucumber.SimpleDb.Async.Linq;

namespace Cucumber.SimpleDb.Async.Session
{
    internal sealed class ProxySimpleDbDomain : ISimpleDbDomain
    {
        private readonly IInternalContext _context;
        private readonly Dictionary<string, ISimpleDbDomain> _loadedDomains;
        private readonly string _name;
        private RealSimpleDbDomain _realDomain;

        internal ProxySimpleDbDomain(string name, Dictionary<string, ISimpleDbDomain> loadedDomains, IInternalContext context)
        {
            _name = name;
            _loadedDomains = loadedDomains;
            _context = context;
        }

        private async Task<ISimpleDbDomain> GetRealDomainAsync()
        {
            if (_realDomain == null)
            {
                await LoadRealDomainAsync().ConfigureAwait(false);
            }
            return _realDomain;
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

        public async Task<long> GetAttributeNameCountAsync()
        {
            return await (await GetRealDomainAsync().ConfigureAwait(false)).GetAttributeNameCountAsync().ConfigureAwait(false);
        }

        public async Task<long> GetAttributeValueCountAsync()
        {
            return await (await GetRealDomainAsync().ConfigureAwait(false)).GetAttributeValueCountAsync().ConfigureAwait(false);
        }

        public async Task<long> GetTotalItemNameSizeAsync()
        {
            return await (await GetRealDomainAsync().ConfigureAwait(false)).GetTotalItemNameSizeAsync().ConfigureAwait(false);
        }

        public async Task<long> GetTotalAttributeValueSizeAsync()
        {
            return await (await GetRealDomainAsync().ConfigureAwait(false)).GetTotalAttributeValueSizeAsync().ConfigureAwait(false);
        }

        public async Task<long> GetTotalAttributeNameSizeAsync()
        {
            return await (await GetRealDomainAsync().ConfigureAwait(false)).GetTotalAttributeNameSizeAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync()
        {
            await _context.Service.DeleteDomainAsync(_name).ConfigureAwait(false); //TODO: switch to deferred/command pattern
            if (_loadedDomains != null)
            {
                _loadedDomains.Remove(_name);
            }
            _realDomain = null;
        }

        private async Task LoadRealDomainAsync()
        {
            var data = await _context.Service.GetDomainMetaAsync(_name).ConfigureAwait(false);

            _realDomain = new RealSimpleDbDomain(_context, _name, data);
        }
    }
}