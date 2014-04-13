using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cucumber.SimpleDb.Async.Linq;

namespace Cucumber.SimpleDb.Async.Session
{
    internal sealed class SessionSimpleDbItemCollection : SimpleDbQuery<ISimpleDbItem>, ISimpleDbItemCollection
    {
        private readonly IInternalContext _context;
        private readonly ISimpleDbDomain _domain;
        private readonly Dictionary<string, ISimpleDbItem> _fetchedItems;

        internal SessionSimpleDbItemCollection(IInternalContext context, ISimpleDbDomain domain, IAsyncQueryProvider queryProvider)
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

        public async Task<ISimpleDbItem> GetItemAsync(string name)
        {
            if (_fetchedItems.ContainsKey(name) == false)
            {
                var element = await _context.Service.GetAttributesAsync(_domain.Name, name, false).ConfigureAwait(false);
                if (element.Descendants("GetAttributesResult").First().HasElements)
                {
                    _fetchedItems.Add(name, new SessionSimpleDbItem(_context, _domain, name, element, true));
                }
                else
                {
                    return null;
                }
            }
            return _fetchedItems[name];
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