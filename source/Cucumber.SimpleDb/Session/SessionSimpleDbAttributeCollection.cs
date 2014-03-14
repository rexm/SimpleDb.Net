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

        internal SessionSimpleDbAttributeCollection(IInternalContext context, SessionSimpleDbItem item, XElement data, bool complete)
        {
            _item = item;
            _complete = complete;
            try
            {
                _attributes = data.Descendants("Attribute").Select(
                        x => new SessionSimpleDbAttribute(
                            _item,
                            x.Element("Name").Value,
                            x.Elements("Value").Select(val => val.Value).ToArray()
                        )
                    ).ToDictionary(
                        att => att.Name,
                        att => att
                    );
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
                if (!_complete)
                {
                    throw new NotImplementedException(
                        "Lazy-completing partially hydrated items is not yet supported");
                }
                if (!this.HasAttribute(attributeName))
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
            if (!_complete)
            {
                throw new NotImplementedException(
                    "Lazy-completing partially hydrated items is not yet supported");
            }
            return _attributes.ContainsKey(attributeName);
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
    }
}
