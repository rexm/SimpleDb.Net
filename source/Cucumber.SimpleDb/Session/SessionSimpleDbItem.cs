using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionSimpleDbItem : ISimpleDbItem, ISessionItem
    {
        private static readonly XNamespace SdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";
        private readonly SessionSimpleDbAttributeCollection _attributes;
        private readonly ISimpleDbDomain _domain;
        private readonly string _name;
        private bool _deleted;

        internal SessionSimpleDbItem(IInternalContext context, ISimpleDbDomain domain, XElement data, bool complete)
            : this(context, domain, data.Element(SdbNs + "Name").Value, data, complete)
        {
        }

        internal SessionSimpleDbItem(IInternalContext context, ISimpleDbDomain domain, string name, XContainer data, bool complete)
        {
            _domain = domain;
            try
            {
                _name = name;
                _attributes = new SessionSimpleDbAttributeCollection(this, data, complete);
            }
            catch (Exception ex)
            {
                throw new SimpleDbException("The response from SimpleDB was not valid", ex);
            }
            context.Session.Attach(this);
        }

        IEnumerable<ISimpleDbAttribute> ISessionItem.Attributes
        {
            get { return _attributes.Where(att => ((ISessionAttribute) att).IsDirty); }
        }

        SessionItemState ISessionItem.State
        {
            get
            {
                if (_deleted)
                {
                    return SessionItemState.Delete;
                }
                return ((ISessionItem) this).Attributes.Any() ? SessionItemState.Update : SessionItemState.Unchanged;
            }
        }

        ISimpleDbDomain ISessionItem.Domain
        {
            get { return _domain; }
        }

        public string Name
        {
            get { return _name; }
        }

        public ISimpleDbAttributeCollection Attributes
        {
            get { return _attributes; }
        }

        public SimpleDbAttributeValue this[string attributeName]
        {
            get
            {
                return !_attributes.HasAttribute(attributeName) ? null : _attributes[attributeName].Value;
            }
            set
            {
                if (!_attributes.HasAttribute(attributeName))
                {
                    _attributes.Add(attributeName, value);
                }
                else
                {
                    _attributes[attributeName].Value = value;
                }
            }
        }

        public void Delete()
        {
            _deleted = true;
        }

        public void DeleteWhen(string attributeName, SimpleDbAttributeValue expectedValue)
        {
            throw new NotImplementedException();
        }
    }
}