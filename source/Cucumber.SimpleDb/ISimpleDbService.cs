﻿using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Provides a transport-layer implementation against the underlying AWS API
    /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API.html
    /// </summary>
    public interface ISimpleDbService
    {
        /// <summary>
        /// Deletes the specified attributes in batches.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_BatchDeleteAttributes.html
        /// </summary>
        /// <returns>Operation metadata.</returns>
        /// <param name="domain">Domain.</param>
        /// <param name="items">Items.</param>
        Task<XElement> BatchDeleteAttributesAsync(string domain, params object[] items);

        /// <summary>
        /// Puts the specified attributes in batches.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_BatchPutAttributes.html
        /// </summary>
        /// <returns>Operation metadata.</returns>
        /// <param name="domain">Domain.</param>
        /// <param name="items">Items.</param>
        Task<XElement> BatchPutAttributesAsync(string domain, params object[] items);

        /// <summary>
        /// Creates the domain.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_CreateDomain.html
        /// </summary>
        /// <returns>Operation metadata.</returns>
        /// <param name="domain">The unique domain name.</param>
        Task<XElement> CreateDomainAsync(string domain);

        /// <summary>
        /// Deletes the domain.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_DeleteDomain.html
        /// </summary>
        /// <returns>Operation metadata.</returns>
        /// <param name="domain">The unique domain name.</param>
        Task<XElement> DeleteDomainAsync(string domain);

        /// <summary>
        /// Deletes the specified attributes for the specified item.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_DeleteAttributes.html
        /// </summary>
        /// <returns>Operation metadata.</returns>
        /// <param name="domain">Domain.</param>
        /// <param name="itemName">Item name.</param>
        /// <param name="attributes">Attributes to delete.</param>
        Task<XElement> DeleteAttributesAsync(string domain, string itemName, params object[] attributes);

        /// <summary>
        /// Gets the domain meta.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_DomainMetadata.html
        /// </summary>
        /// <returns>The domain meta.</returns>
        /// <param name="domain">The unique domain name.</param>
        Task<XElement> GetDomainMetaAsync(string domain);

        /// <summary>
        /// Lists the domains.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_ListDomains.html
        /// </summary>
        /// <returns>The domains.</returns>
        Task<XElement> ListDomainsAsync();

        /// <summary>
        /// Lists the domains.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_ListDomains.html
        /// </summary>
        /// <returns>The domains.</returns>
        /// <param name="nextPageToken">Next page token.</param>
        Task<XElement> ListDomainsAsync(string nextPageToken);

        /// <summary>
        /// Puts the specified attributes to the specified item.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_PutAttributes.html
        /// </summary>
        /// <returns>Operation metadata.</returns>
        /// <param name="domain">The unique domain name.</param>
        /// <param name="name">The unique item name.</param>
        /// <param name="attributes">The attributes to put.</param>
        Task<XElement> PutAttributesAsync(string domain, string name, params object[] attributes);

        /// <summary>
        /// Gets the specified attributes for the specified item.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_GetAttributes.html
        /// </summary>
        /// <returns>The attributes.</returns>
        /// <param name="domain">The unique domain name.</param>
        /// <param name="name">The unique item name.</param>
        /// <param name="useConsistency">If set to <c>true</c> use consistency.</param>
        /// <param name="attributeNames">The attribute names.</param>
        Task<XElement> GetAttributesAsync(string domain, string name, bool useConsistency, params string[] attributeNames);

        /// <summary>
        /// Gets results for the specified query.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_Select.html
        /// </summary>
        /// <returns>The resulting attribute set.</returns>
        /// <param name="query">The query.</param>
        /// <param name="useConsistency">If set to <c>true</c> use consistency.</param>
        Task<XElement> SelectAsync(string query, bool useConsistency);

        /// <summary>
        /// Gets results for the specified query.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_Select.html
        /// </summary>
        /// <returns>The resulting attribute set.</returns>
        /// <param name="query">The query.</param>
        /// <param name="useConsistency">If set to <c>true</c> use consistency.</param>
        /// <param name="nextPageToken">Next page token.</param>
        Task<XElement> SelectAsync(string query, bool useConsistency, string nextPageToken);
    }
}