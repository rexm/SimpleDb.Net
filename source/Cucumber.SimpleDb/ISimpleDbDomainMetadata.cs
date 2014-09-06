using System;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// The detailed metadata for the domain.
    /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_DomainMetadata.html
    /// </summary>
    public interface ISimpleDbDomainMetadata
    {
        /// <summary>
        /// Gets UTC date and time the metadata was calculated.
        /// </summary>
        DateTime Calculated { get; }

        /// <summary>
        /// Gets the total count the items in the current domain.
        /// </summary>
        long ItemCount { get; }

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

