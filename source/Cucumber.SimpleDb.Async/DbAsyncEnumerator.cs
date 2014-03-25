using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Async
{
    internal class DbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly Func<CancellationToken, Task<IEnumerable<T>>> _getResultAsync;
        private IEnumerator<T> _results;
        private bool _isInitialized;

        public DbAsyncEnumerator(Func<CancellationToken, Task<IEnumerable<T>>> getResultAsync)
        {
            _getResultAsync = getResultAsync;
        }

        public void Dispose()
        {
        }

        public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            if (!_isInitialized)
            {
                _results = (await _getResultAsync(cancellationToken).ConfigureAwait(false)).GetEnumerator();
                _isInitialized = true;
            }

            return _results.MoveNext();
        }

        public T Current
        {
            get { return _results.Current; }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }
}