using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public int Count
        {
            get
            {
                if (_domains == null)
                {
                    PopulateDomains();
                }
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

        public ISimpleDbDomain Add(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            _session.Service.CreateDomain(name);
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
            if (_domains == null)
            {
                PopulateDomains();
            }
            return _domains.ContainsKey(name);
        }

        public IEnumerator<ISimpleDbDomain> GetEnumerator()
        {
            if (_domains == null)
            {
                PopulateDomains();
            }
            return _domains.Select(kvp => kvp.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void PopulateDomains()
        {
            _domains = new Dictionary<string, ISimpleDbDomain>(StringComparer.OrdinalIgnoreCase);
            string nextPageToken = null;
            do
            {
                var result = _session.Service.ListDomains(nextPageToken);
                foreach (var domainNode in result.Elements("DomainName"))
                {
                    _domains.Add(domainNode.Value, new ProxySimpleDbDomain(domainNode.Value, _domains, _session));
                }
                nextPageToken = result.Elements("NextPageToken").Select(x => x.Value).FirstOrDefault();
            } while (!string.IsNullOrEmpty(nextPageToken));
        }
    }
}