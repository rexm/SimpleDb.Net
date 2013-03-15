using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionSimpleDbItem : ISimpleDbItem, ISessionItem
    {
        private readonly XElement _data;
        private readonly IInternalContext _context;
        private readonly string _name;
        private readonly SessionSimpleDbAttributeCollection _attributes;
        private readonly ISimpleDbDomain _domain;
        private bool _deleted;

        internal SessionSimpleDbItem(IInternalContext context, ISimpleDbDomain domain, XElement data, bool complete)
            : this(context, domain, data.Element("Name").Value, data, complete)
        {
        }

        internal SessionSimpleDbItem(IInternalContext context, ISimpleDbDomain domain, string name, XElement data, bool complete)
        {
            _domain = domain;
            _context = context;
            _data = data;
            try
            {
                _name = name;
                _attributes = new SessionSimpleDbAttributeCollection(_context, this, _data, complete);
            }
            catch (Exception ex)
            {
                throw new SimpleDbException("The response from SimpleDB was not valid", ex);
            }
            _context.Session.Attach(this);
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
                if (!_attributes.HasAttribute(attributeName))
                {
                    return null;
                }
                return _attributes[attributeName].Value;
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

        IEnumerable<ISimpleDbAttribute> ISessionItem.Attributes
        {
            get
            {
                return _attributes.Where(att => ((ISessionAttribute)att).IsDirty);
            }
        }

        SessionItemState ISessionItem.State
        {
            get
            {
                if (_deleted)
                {
                    return SessionItemState.Delete;
                }
                return ((ISessionItem)this).Attributes.Count() > 0 ? SessionItemState.Update : SessionItemState.Unchanged;
            }
        }

        ISimpleDbDomain ISessionItem.Domain
        {
            get { return _domain; }
        }
    }
}
