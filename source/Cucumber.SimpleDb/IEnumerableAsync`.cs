using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Allows an enumerable sequence to be retrieved asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of objects in the sequence.</typeparam>
    internal interface IEnumerableAsync<T>
    {
        /// <summary>
        /// Asynchronously gets an enumerable sequence.
        /// </summary>
        /// <returns>
        /// A task that can contain an enumerable sequence.
        /// </returns>
        Task<IEnumerable<T>> GetEnumerableAsync();
    }
}