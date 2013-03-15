using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly TKey _key;
        private readonly IEnumerable<TElement> _group;

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
