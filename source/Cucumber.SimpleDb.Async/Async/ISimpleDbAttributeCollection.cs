using System.Collections.Generic;

namespace Cucumber.SimpleDb.Async
{
    /// <summary>
    /// Represents the collection of <c>Cucumber.SimpleDb.ISimpleDbAttribute</c> instances on a <c>Cucumber.SimpleDb.ISimpleDbItem</c> instance
    /// </summary>
    public interface ISimpleDbAttributeCollection : IEnumerable<ISimpleDbAttribute>
    {
        /// <summary>
        /// Gets the attribute of the specified name.
        /// </summary>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the collection does not contain an attribute with a matching name</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="attributeName"/> is null or empty</exception>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The requested attribute</returns>
        ISimpleDbAttribute this[string attributeName] { get; }

        /// <summary>
        /// Gets whether the collection contains an attribute with the speified name
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="attributeName"/> is null or empty</exception>
        /// <param name="attributeName">The name of the attribute to search for</param>
        /// <returns>True if an attribute with the specified name exists; otherwise false</returns>
        bool HasAttribute(string attributeName);

        /// <summary>
        /// Adds an attribute to the collection.
        /// <para>If the collection already contains an attribute with the same name, that attribute is overwritten</para>
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="attributeName"/> or <paramref name="value"/> is null or empty</exception>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="value">The initial value of the attribute</param>
        void Add(string attributeName, SimpleDbAttributeValue value);
    }
}