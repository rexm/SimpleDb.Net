using System;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Async
{
    /// <summary>
    /// Represents the main entry point for SimpleDb.NET
    /// </summary>
    public interface ISimpleDbContext : IDisposable
    {
        /// <summary>
        /// Gets the collection of domains residing in this SimpleDb instance.
        /// </summary>
        Task<ISimpleDbDomainCollection> GetDomainsAsync();

        /// <summary>
        /// Gets the domain residing in this SimpleDb instance with the given name.
        /// </summary>
        /// <param name="name"></param>
        Task<ISimpleDbDomain> GetDomainAsync(string name);

        /// <summary>
        /// Computes the set of modified objects to be inserted, updated, or deleted, and executes the appropriate commands to implement the changes to SimpleDb.
        /// </summary>
        Task SubmitChangesAsync();
    }
}