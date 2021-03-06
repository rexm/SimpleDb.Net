﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cucumber.SimpleDb.Utilities;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionNewSimpleDbItem : SessionSimpleDbItem, ISessionItem
    {
        private static readonly XNamespace sdbNs = "http://sdb.amazonaws.com/doc/2009-04-15/";

        internal SessionNewSimpleDbItem(IInternalContext context, ISimpleDbDomain domain, string name, Dictionary<string, SimpleDbAttributeValue> values)
            : base(context, domain, name, ToXElement(values), true)
        {
            context.Session.Attach (this);
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
                return new XElement(sdbNs + "Item");
            }
            return new XElement(sdbNs + "Item",
                values
                    .Select(kvp => new XElement(sdbNs + "Attribute",
                    new[] { new XElement(sdbNs + "Name", kvp.Key) }
                        .Concat(kvp.Value.Values
                            .Select(val => new XElement(sdbNs + "Value", val)))))
                );
        }
    }
}
