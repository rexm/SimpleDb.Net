using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Cucumber.SimpleDb.ServiceBus;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionSimpleDbAttribute : ISimpleDbAttribute, ISessionAttribute
    {
        private readonly SimpleDbAttributeValue _originalValue;
        private SimpleDbAttributeValue _newValue;
        private readonly SessionSimpleDbItem _item;
        private readonly string _name;

        internal SessionSimpleDbAttribute(SessionSimpleDbItem item, string name, params string[] values)
        {
            _item = item;
            _name = name;
            _originalValue = new SimpleDbAttributeValue(values);
            _newValue = _originalValue;
        }

        public string Name
        {
            get { return _name; }
        }

        public SimpleDbAttributeValue Value
        {
            get
            {
                return _newValue;
            }
            set
            {
                _newValue = value;
				this.IsDirty = _newValue.Values.SequenceEqual (_originalValue.Values);
            }
        }

        public bool IsDirty
        {
            get;
            private set;
        }
    }
}
