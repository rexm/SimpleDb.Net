using System.Collections;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Async.Infrastructure
{
    /// <summary>
    /// Version of the <see cref="T:System.Collections.Generic.IEnumerable`1"/> interface that allows the enumerable sequence to be retrieved asynchronously.
    /// </summary>
    public interface IEnumerableAsync
    {
        /// <summary>
        /// Asynchronously gets an enumerator that can be used to enumerate the sequence.
        /// </summary>
        /// <returns>
        /// A task that can contain an enumerator for enumeration over the sequence.
        /// </returns>
        Task<IEnumerator> GetEnumeratorAsync();
    }
}