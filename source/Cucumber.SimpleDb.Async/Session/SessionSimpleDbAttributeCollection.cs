using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Async.Session
{
    internal sealed class SessionSimpleDbAttributeCollection : ISimpleDbAttributeCollection
    {
        private static readonly XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly Dictionary<string, SessionSimpleDbAttribute> _attributes = new Dictionary<string, SessionSimpleDbAttribute>();
        private readonly bool _complete;
        private readonly SessionSimpleDbItem _item;

        internal SessionSimpleDbAttributeCollection(SessionSimpleDbItem item, XContainer data, bool complete)
        {
            _item = item;
            _complete = complete;
            try
            {
                _attributes = data.Descendants(SdbNs + "Attribute").Select(
                    x => new SessionSimpleDbAttribute(x.Element(SdbNs + "Name").Value,
                        x.Elements(SdbNs + "Value").Select(val => val.Value).ToArray()
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
                if (HasAttribute(attributeName) == false)
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
            if (_attributes.ContainsKey(attributeName) == false)
            {
                if (_complete == false)
                {
                    throw new NotImplementedException(
                        "Lazy-completing partially hydrated items is not yet supported");
                }
                return false;
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
            var attribute = new SessionSimpleDbAttribute(attributeName)
            {
                Value = value
            };
            if (_attributes.ContainsKey(attributeName))
            {
                _attributes.Remove(attributeName);
            }
            _attributes.Add(attributeName, attribute);
        }

        public IEnumerator<ISimpleDbAttribute> GetEnumerator()
        {
            return _attributes.Select(kvp => kvp.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(string attributeName)
        {
            return _attributes.Remove(attributeName);
        }
    }
}