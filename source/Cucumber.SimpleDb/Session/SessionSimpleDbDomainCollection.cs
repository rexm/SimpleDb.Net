using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;
using System.Collections;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Session
{
    internal sealed class SessionSimpleDbDomainCollection : ISimpleDbDomainCollection
    {
        private Dictionary<string, ISimpleDbDomain> _domains;
        private IInternalContext _session;

        internal SessionSimpleDbDomainCollection(IInternalContext session)
        {
            _session = session;
        }

        public int Count
        {
            get
            {
                if (_domains == null)
                {
                    _domains = GetDomainDictionaryAsync().Result;
                }
                return this._domains.Count;
            }
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
                    return new SessionSimpleDbDomain(name, this._domains, _session);
                }
                throw new KeyNotFoundException(
                    string.Format("A domain named '{0}' does not exist",
                    name));
            }
        }

        public ISimpleDbDomain Add(string name)
        {
            return AddAsync(name).Result;
        }

        public bool HasDomain(string name)
        {
            return HasDomainAsync(name).Result;
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
                _domains.Add(name, new SessionSimpleDbDomain(name, _domains, _session));
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
                foreach (var domainNode in result.Elements(SdbNs + "DomainName"))
                {
                    domains.Add(domainNode.Value, new SessionSimpleDbDomain(domainNode.Value, domains, _session));
                }
                nextPageToken = result.Elements(SdbNs + "NextPageToken").Select(x => x.Value).FirstOrDefault();
            } while (!string.IsNullOrEmpty(nextPageToken));
            return domains;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly static XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
    }
}
