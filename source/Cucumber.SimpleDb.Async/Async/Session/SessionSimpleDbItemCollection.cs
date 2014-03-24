using System;
using System.Collections.Generic;
using System.Linq;

namespace Cucumber.SimpleDb.Async.Session
{
    internal class SessionSimpleDbItemCollection : Query<ISimpleDbItem>, ISimpleDbItemCollection
    {
        private readonly IInternalContext _context;
        private readonly ISimpleDbDomain _domain;
        private readonly Dictionary<string, ISimpleDbItem> _fetchedItems;

        internal SessionSimpleDbItemCollection(IInternalContext context, ISimpleDbDomain domain, IQueryProvider queryProvider)
            : base(queryProvider)
        {
            _context = context;
            _domain = domain;
            _fetchedItems = new Dictionary<string, ISimpleDbItem>();
        }

        public ISimpleDbDomain Domain
        {
            get { return _domain; }
        }

        public int Count
        {
            get { return this.Count(); }
        }

        public ISimpleDbItem this[string name]
        {
            get
            {
                if (_fetchedItems.ContainsKey(name) == false)
                {
                    var element = _context.Service.GetAttributes(_domain.Name, name, false);
                    if (element.Descendants("GetAttributesResult").First().HasElements)
                    {
                        _fetchedItems.Add(name,
                            new SessionSimpleDbItem(
                                _context,
                                _domain,
                                name,
                                element,
                                true));
                    }
                    else
                    {
                        return null;
                    }
                }
                return _fetchedItems[name];
            }
        }

        public ISimpleDbItem Add(string name)
        {
            return Add(name, null);
        }

        public ISimpleDbItem Add(string name, Dictionary<string, SimpleDbAttributeValue> values)
        {
            var item = new SessionNewSimpleDbItem(_context, _domain, name, values);
            _fetchedItems.Add(name, item);
            return item;
        }

        public ISimpleDbItem AddWhen(string name, string conditionAttribute, SimpleDbAttributeValue conditionValue)
        {
            throw new NotImplementedException();
        }

        public ISimpleDbItem AddWhen(string name, Dictionary<string, SimpleDbAttributeValue> values, string conditionAttribute, SimpleDbAttributeValue conditionValue)
        {
            throw new NotImplementedException();
        }
    }
}