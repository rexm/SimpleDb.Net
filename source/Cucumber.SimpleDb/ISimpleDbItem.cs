namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Represents a single instance of a SimpleDb item.
    /// </summary>
    public interface ISimpleDbItem
    {
        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the collection of attributes present on the current item.
        /// </summary>
        ISimpleDbAttributeCollection Attributes { get; }

        /// <summary>
        /// Gets and sets the value of the specified attribute.
        /// <para>If the value of a non-existent attribute is requested, null will be returned.</para>
        /// <para>If a value is set for a non-existent attribute, the attribute will be created with the specified name.</para>
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="attributeName"/> is null or empty</exception>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The value of the attribute if it exists; otherwise null.</returns>
        SimpleDbAttributeValue this[string attributeName] { get; set; }

        /// <summary>
        /// Marks the current instance for deletion when <c>Cucumber.SimpleDb.ISimpleDbContext.SubmitChanges()</c> is invoked.
        /// </summary>
        void Delete();

        /// <summary>
        /// Marks the current instance for conditional deletion when <c>Cucumber.SimpleDb.ISimpleDbContext.SubmitChanges()</c> is invoked.
        /// </summary>
        /// <param name="expectedAttribute">The name of the attribute to verify before deleting.</param>
        /// <param name="expectedValue">The expected value of the attribute (or null if only existence should be verified).</param>
        void DeleteWhen(string expectedAttribute, SimpleDbAttributeValue expectedValue);
    }
}