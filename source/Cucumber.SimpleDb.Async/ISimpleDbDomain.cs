namespace Cucumber.SimpleDb.Async
{
    /// <summary>
    /// Represents a single domain instance in SimpleDb.
    /// </summary>
    public interface ISimpleDbDomain
    {
        /// <summary>
        /// Gets the collection of items present in the current domain.
        /// </summary>
        ISimpleDbItemCollection Items { get; }

        /// <summary>
        /// Gets the name of the domain.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the total number of attribute names present in the current domain.
        /// </summary>
        long AttributeNameCount { get; }

        /// <summary>
        /// Gets the total number of attribute values present in the current domain.
        /// </summary>
        long AttributeValueCount { get; }

        /// <summary>
        /// Gets the total size (in bytes) of the item names in the current domain.
        /// </summary>
        long TotalItemNameSize { get; }

        /// <summary>
        /// Gets the total size (in bytes) of the attribute values in the current domain.
        /// </summary>
        long TotalAttributeValueSize { get; }

        /// <summary>
        /// Gets the total size (in bytes) of the attribute names in the current domain.
        /// </summary>
        long TotalAttributeNameSize { get; }

        /// <summary>
        /// Deletes the current domain and all its data.
        /// </summary>
        void Delete();
    }
}