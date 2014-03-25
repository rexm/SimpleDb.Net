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
        private readonly Dictionary<string, ISimpleDbDomain> _domains;

        internal SessionSimpleDbDomainCollection(IInternalContext session, Dictionary<string, ISimpleDbDomain> domains)
        {
            _session = session;
            _domains = domains;
        }

        public int Count
        {
            get
            {
                return _domains.Count;
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
                    return new ProxySimpleDbDomain(name, _domains, _session);
                }
                throw new KeyNotFoundException(
                    string.Format("A domain named '{0}' does not exist",
                        name));
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

        public bool HasDomain(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return _domains.ContainsKey(name);
        }

        public IEnumerator<ISimpleDbDomain> GetEnumerator()
        {
            return _domains.Select(kvp => kvp.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}