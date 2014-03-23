using System;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Represents the main entry point for SimpleDb.NET
    /// </summary>
    public interface ISimpleDbContext : IDisposable
    {
        /// <summary>
        /// Gets the collection of domains residing in this SimpleDb instance.
        /// </summary>
        ISimpleDbDomainCollection Domains { get; }

        /// <summary>
        /// Computes the set of modified objects to be inserted, updated, or deleted, and executes the appropriate commands to implement the changes to SimpleDb.
        /// </summary>
        void SubmitChanges();
    }
}