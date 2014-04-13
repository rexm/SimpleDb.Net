using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Async.Session
{
    internal sealed class SessionSimpleDbDomainCollection : ISimpleDbDomainCollection
    {
        private readonly IInternalContext _session;
        private Dictionary<string, ISimpleDbDomain> _domains;

        internal SessionSimpleDbDomainCollection(IInternalContext session)
        {
            _session = session;
        }

        public ISimpleDbDomain this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }
                if (_domains != null)
                {
                    if (_domains.ContainsKey(name))
                    {
                        return _domains[name];
                    }
                }
                else
                {
                    return new ProxySimpleDbDomain(name, _domains, _session);
                }
                throw new KeyNotFoundException(string.Format("A domain named '{0}' does not exist", name));
            }
        }

        public async Task<ISimpleDbDomain> AddAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            await _session.Service.CreateDomainAsync(name).ConfigureAwait(false);
            if (_domains != null)
            {
                _domains.Add(name, new ProxySimpleDbDomain(name, _domains, _session));
            }
            return this[name];
        }

        public async Task<bool> HasDomainAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (_domains == null)
            {
                _domains = await GetDomainDictionaryAsync();
            }
            return _domains.ContainsKey(name);
        }

        public IEnumerator<ISimpleDbDomain> GetEnumerator()
        {
            return _domains != null 
                ? _domains.Select(kvp => kvp.Value).GetEnumerator() 
                : Enumerable.Empty<ISimpleDbDomain>().GetEnumerator();
        }

        private async Task<Dictionary<string, ISimpleDbDomain>> GetDomainDictionaryAsync()
        {
            var domains = new Dictionary<string, ISimpleDbDomain>(StringComparer.OrdinalIgnoreCase);
            string nextPageToken = null;
            do
            {
                var result = await _session.Service.ListDomainsAsync(nextPageToken).ConfigureAwait(false);
                foreach (var domainNode in result.Elements("DomainName"))
                {
                    domains.Add(domainNode.Value, new ProxySimpleDbDomain(domainNode.Value, domains, _session));
                }
                nextPageToken = result.Elements("NextPageToken").Select(x => x.Value).FirstOrDefault();
            } while (!string.IsNullOrEmpty(nextPageToken));
            return domains;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}