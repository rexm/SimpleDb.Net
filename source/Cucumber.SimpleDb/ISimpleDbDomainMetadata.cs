using System;

namespace Cucumber.SimpleDb
{
    public interface ISimpleDbDomainMetadata
    {
        /// <summary>
        /// Gets the total count the items in the current domain.
        /// </summary>
        long TotalItemCount { get; }

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
    }
}

