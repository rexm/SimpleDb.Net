using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Represents a collection of <c>Cucumber.SimpleDb.ISimpleDbDomain</c> instances
    /// </summary>
    public interface ISimpleDbDomainCollection : IEnumerable<ISimpleDbDomain>
    {
        /// <summary>
        /// Gets the total number of domains in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the domain of the specified name.
        /// </summary>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the collection does not contain an domain with a matching name</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="name"/> is null or empty</exception>
        /// <param name="name">The name of the domain</param>
        /// <returns>The requested domain</returns>
        ISimpleDbDomain this[string name] { get; }

        /// <summary>
        /// Creates a domain with the specified name.
        /// <para>If a domain with the same name already exists, no domain is created and the existing domain is returned.</para>
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="name"/> is null or empty</exception>
        /// <param name="name">The name of the domain to create</param>
        /// <returns>The new domain if no matching domain existed, otherwise the existing domain</returns>
        ISimpleDbDomain Add(string name);

        /// <summary>
        /// Gets whether a domain with the speified name exists
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="attributeName"/> is null or empty</exception>
        /// <param name="name">The name of the domain to search for</param>
        /// <returns>True if a domain with the specified name exists; otherwise false</returns>
        bool HasDomain(string name);
    }
}
