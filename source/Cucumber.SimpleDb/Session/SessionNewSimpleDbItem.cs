using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cucumber.SimpleDb.Utilities;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionNewSimpleDbItem : SessionSimpleDbItem, ISessionItem
    {
        internal SessionNewSimpleDbItem(IInternalContext context, ISimpleDbDomain domain, string name)
            : this(context, domain, name, Enumerable.Empty<KeyValuePair<string, SimpleDbAttributeValue>>().ToDictionary())
        {
        }

        internal SessionNewSimpleDbItem(IInternalContext context, ISimpleDbDomain domain, string name, Dictionary<string, SimpleDbAttributeValue> values)
            : base(context, domain, name, ToXElement(values), true)
        {
        }

        SessionItemState ISessionItem.State
        {
            get { return SessionItemState.Create; }
        }

        IEnumerable<ISimpleDbAttribute> ISessionItem.Attributes
        {
            get { return this.Attributes; }
        }

        private static XElement ToXElement(Dictionary<string, SimpleDbAttributeValue> values)
        {
            if (values == null)
            {
                return new XElement("Item");
            }
            return new XElement("Item",
                values
                    .Select(kvp => new XElement("Attribute",
                        new[]{new XElement("Name", kvp.Key)}
                        .Concat(kvp.Value.Values
                            .Select(val => new XElement("Value", val)))))
                );
        }
    }
}
