using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Session
{
    internal sealed class SessionSimpleDbAttributeCollection : ISimpleDbAttributeCollection
    {
        private readonly SessionSimpleDbItem _item;
        private Dictionary<string, SessionSimpleDbAttribute> _attributes = new Dictionary<string, SessionSimpleDbAttribute>();
        private bool _complete;
        private static readonly XNamespace sdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly IInternalContext _context;

        internal SessionSimpleDbAttributeCollection(IInternalContext context, SessionSimpleDbItem item, XElement data, bool complete)
        {
            _context = context;
            _item = item;
            _complete = complete;

            var result = new Dictionary<string, SessionSimpleDbAttribute>();

            try
            {
                AddAttributeData(data);
            }
            catch (Exception ex)
            {
                throw new SimpleDbException("The response from SimpleDB was not valid", ex);
            }
        }

        public ISimpleDbAttribute this[string attributeName]
        {
            get
            {
                if (string.IsNullOrEmpty(attributeName))
                {
                    throw new ArgumentNullException("attributeName");
                }
                if (this.HasAttribute(attributeName) == false)
                {
                    throw new KeyNotFoundException(
                        string.Format("The attribute '{0}' is not present on the item '{1}'",
                            attributeName,
                            _item.Name));
                }
                return _attributes[attributeName];
            }
        }

        public bool HasAttribute(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException("attributeName");
            }
            if (_attributes.ContainsKey (attributeName) == false)
            {
                if (_complete == false)
                {
                    AddAttributeData(_context.Service.GetAttributes(((ISessionItem)_item).Domain.Name, _item.Name, false));
                    _complete = true;
                }
                return _attributes.ContainsKey (attributeName);
            }
            return true;
        }

        public void Add(string attributeName, SimpleDbAttributeValue value)
        {
            if (string.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException("attributeName");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var attribute = new SessionSimpleDbAttribute(_item, attributeName);
            attribute.Value = value;
            if (_attributes.ContainsKey(attributeName))
            {
                _attributes.Remove(attributeName);
            }
            _attributes.Add(attributeName, attribute);
        }

        public bool Remove(string attributeName)
        {
            return _attributes.Remove(attributeName);
        }

        public IEnumerator<ISimpleDbAttribute> GetEnumerator()
        {
            return _attributes.Select(kvp => kvp.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void AddAttributeData(XElement data)
        {
            var subResult = data.Descendants(sdbNs + "Attribute").Select(x => new SessionSimpleDbAttribute(_item, x.Element(sdbNs + "Name").Value, x.Elements(sdbNs + "Value").Select(val => val.Value).ToArray()));
            foreach (var resItem in subResult)
            {
                if (!_attributes.ContainsKey(resItem.Name))
                {
                    _attributes.Add(resItem.Name, resItem);
                }
                else
                {
                    _attributes[resItem.Name].Value = new SimpleDbAttributeValue(_attributes[resItem.Name].Value.Values.Concat(resItem.Value.Values).ToArray());
                }
            }
        }
    }
}
