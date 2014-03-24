using System;
using System.Collections.Generic;
using System.Linq;
using Cucumber.SimpleDb.Async.Utilities;

namespace Cucumber.SimpleDb.Async.Session
{
    internal class SimpleDbSession : ISession
    {
        private readonly ISimpleDbService _service;
        private readonly HashSet<ISessionItem> _trackedItems;

        public SimpleDbSession(ISimpleDbService service)
        {
            _service = service;
            _trackedItems = new HashSet<ISessionItem>();
        }

        public void Attach(ISessionItem item)
        {
            _trackedItems.Add(item);
        }

        public void Detatch(ISessionItem item)
        {
            _trackedItems.Remove(item);
        }

        public void SubmitChanges()
        {
            foreach (var operation in CollectOperations())
            {
                operation(_service);
            }
        }

        private IEnumerable<Action<ISimpleDbService>> CollectOperations()
        {
            var domainSets = _trackedItems.GroupBy(item => item.Domain, (x, y) => x.Name == y.Name);
            var enumerable = domainSets as IList<IGrouping<ISimpleDbDomain, ISessionItem>> ?? domainSets.ToList();
            foreach (var action in enumerable.SelectMany(CollectDeleteOperations))
            {
                yield return action;
            }
            foreach (var action in enumerable.SelectMany(CollectUpsertOperations))
            {
                yield return action;
            }
        }

        private static IEnumerable<Action<ISimpleDbService>> CollectDeleteOperations(IGrouping<ISimpleDbDomain, ISessionItem> domainSet)
        {
            var deleteBatches = domainSet.Where(i => i.State == SessionItemState.Delete).GroupsOf(25);
            return deleteBatches.Select(deleteBatch => (Action<ISimpleDbService>) (service => service.BatchDeleteAttributes(domainSet.Key.Name, deleteBatch.Cast<object>().ToArray())));
        }

        private static IEnumerable<Action<ISimpleDbService>> CollectUpsertOperations(IGrouping<ISimpleDbDomain, ISessionItem> domainSet)
        {
            var putBatches = domainSet.Where(i => i.State == SessionItemState.Create || i.State == SessionItemState.Update).GroupsOf(25);
            return putBatches.Select(putBatch => (Action<ISimpleDbService>) (service => service.BatchPutAttributes(domainSet.Key.Name, putBatch.Cast<object>().ToArray())));
        }
    }
}