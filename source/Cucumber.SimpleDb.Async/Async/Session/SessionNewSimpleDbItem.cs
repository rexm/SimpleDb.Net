using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Cucumber.SimpleDb.Async.Session
{
    internal class SessionNewSimpleDbItem : SessionSimpleDbItem, ISessionItem
    {
        internal SessionNewSimpleDbItem(IInternalContext context, ISimpleDbDomain domain, string name, Dictionary<string, SimpleDbAttributeValue> values)
            : base(context, domain, name, ToXElement(values), true)
        {
            context.Session.Attach(this);
        }

        SessionItemState ISessionItem.State
        {
            get { return SessionItemState.Create; }
        }

        IEnumerable<ISimpleDbAttribute> ISessionItem.Attributes
        {
            get { return Attributes; }
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
                        new[]
                        {
                            new XElement("Name", kvp.Key)
                        }
                            .Concat(kvp.Value.Values
                                .Select(val => new XElement("Value", val)))))
                );
        }
    }
}