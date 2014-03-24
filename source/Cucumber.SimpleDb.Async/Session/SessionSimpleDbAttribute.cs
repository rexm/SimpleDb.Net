using System.Linq;

namespace Cucumber.SimpleDb.Async.Session
{
    internal class SessionSimpleDbAttribute : ISimpleDbAttribute, ISessionAttribute
    {
        private readonly string _name;
        private readonly SimpleDbAttributeValue _originalValue;
        private SimpleDbAttributeValue _newValue;

        internal SessionSimpleDbAttribute(string name, params string[] values)
        {
            _name = name;
            _originalValue = new SimpleDbAttributeValue(values);
            _newValue = _originalValue;
        }

        public bool Replace
        {
            get { return true; }
        }

        public bool IsDirty { get; private set; }

        public string Name
        {
            get { return _name; }
        }

        public SimpleDbAttributeValue Value
        {
            get { return _newValue; }
            set
            {
                _newValue = value;
                IsDirty = _newValue.Values.SequenceEqual(_originalValue.Values);
            }
        }
    }
}