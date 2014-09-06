using System.Threading.Tasks;

namespace Cucumber.SimpleDb
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
        /// Deletes the current domain and all its data.
        /// </summary>
        void Delete();

        /// <summary>
        /// Deletes the current domain and all its data.
        /// </summary>
        Task DeleteAsync();

        /// <summary>
        /// Gets the detailed info for the domain.
        /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_DomainMetadata.html
        /// </summary>
        /// <returns>The detailed info for the domain.</returns>
        Task<ISimpleDbDomainMetadata> GetDomainInfoAsync();
    }
}