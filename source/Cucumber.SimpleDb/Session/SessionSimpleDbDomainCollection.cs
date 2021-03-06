﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;

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
                    PopulateDomains();
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
                    return new ProxySimpleDbDomain(name, this._domains, _session);
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
            _session.Service.CreateDomain (name);
            if (_domains != null)
            {
                _domains.Add (name, new ProxySimpleDbDomain (name, _domains, _session));
            }
            return this [name];
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
            return this._domains.Select(kvp => kvp.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void PopulateDomains()
        {
            _domains = new Dictionary<string, ISimpleDbDomain>(StringComparer.OrdinalIgnoreCase);
            XElement result = null;
            string nextPageToken = null;
            do
            {
                result = _session.Service.ListDomains(nextPageToken).Element(SdbNs + "ListDomainsResult");
                foreach (var domainNode in result.Elements(SdbNs + "DomainName"))
                {
                    _domains.Add(domainNode.Value, new ProxySimpleDbDomain(domainNode.Value, _domains, _session));
                }
                nextPageToken = result.Elements(SdbNs + "NextToken").Select(x => x.Value).FirstOrDefault();
            } while (!string.IsNullOrEmpty(nextPageToken));
        }

        private readonly static XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
    }
}
