using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cucumber.SimpleDb.Async.Linq.Structure
{
    internal sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> _group;
        private readonly TKey _key;

        public Grouping(TKey key, IEnumerable<TElement> group)
        {
            _key = key;
            _group = group;
        }

        public TKey Key
        {
            get { return _key; }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _group.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _group.GetEnumerator();
        }
    }
}