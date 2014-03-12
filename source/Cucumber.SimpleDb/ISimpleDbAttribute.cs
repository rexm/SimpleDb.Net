using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Represents a single attribute on a <c>Cucumber.SimpleDb.ISimpleDbItem</c> instance
    /// </summary>
    public interface ISimpleDbAttribute
    {
        /// <summary>
        /// Gets and sets the value of the attribute
        /// </summary>
        SimpleDbAttributeValue Value { get; set; }

        /// <summary>
        /// Gets the name of the attribute
        /// </summary>
        string Name { get; }
    }
}
