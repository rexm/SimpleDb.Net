using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Async.Session
{
    internal class SessionSimpleDbContext : ISimpleDbContext, IInternalContext
    {
        private readonly ISimpleDbService _service;
        private readonly ISession _session;

        internal SessionSimpleDbContext(ISimpleDbService service, ISession session)
        {
            _service = service;
            _session = session;
        }

        ISimpleDbService IInternalContext.Service
        {
            get { return _service; }
        }

        ISession IInternalContext.Session
        {
            get { return _session; }
        }

        public async Task<ISimpleDbDomainCollection> GetDomainsAsync()
        {
            return new SessionSimpleDbDomainCollection(this, await GetDomainDictionaryAsync().ConfigureAwait(false));
        }

        public async Task<ISimpleDbDomain> GetDomainAsync(string name)
        {
            return (await GetDomainDictionaryAsync().ConfigureAwait(false))[name];
        }

        private async Task<Dictionary<string, ISimpleDbDomain>> GetDomainDictionaryAsync()
        {
            var domains = new Dictionary<string, ISimpleDbDomain>(StringComparer.OrdinalIgnoreCase);
            string nextPageToken = null;
            do
            {
                var result = await ((IInternalContext)this).Service.ListDomainsAsync(nextPageToken).ConfigureAwait(false);
                foreach (var domainNode in result.Elements("DomainName"))
                {
                    domains.Add(domainNode.Value, new ProxySimpleDbDomain(domainNode.Value, domains, this));
                }
                nextPageToken = result.Elements("NextPageToken").Select(x => x.Value).FirstOrDefault();
            } while (!string.IsNullOrEmpty(nextPageToken));
            return domains;
        }

        public async Task SubmitChangesAsync()
        {
            await _session.SubmitChangesAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
        }
    }
}