using System;
using System.Collections.Generic;
using System.Linq;
using Cucumber.SimpleDb.Utilities;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Session
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

        public async Task SubmitChangesAsync()
        {
            foreach (var operation in CollectAsyncOperations())
            {
                await operation(_service).ConfigureAwait(false);
            }
        }

        private IEnumerable<Func<ISimpleDbService, Task>> CollectAsyncOperations()
        {
            var domainSets = _trackedItems.GroupBy(item => item.Domain, (x, y) => x.Name == y.Name);
            var enumerable = domainSets as IList<IGrouping<ISimpleDbDomain, ISessionItem>> ?? domainSets.ToList();
            foreach (var action in enumerable.SelectMany(CollectAsyncDeleteOperations))
            {
                yield return action;
            }
            foreach (var action in enumerable.SelectMany(CollectAsyncUpsertOperations))
            {
                yield return action;
            }
        }

        private static IEnumerable<Func<ISimpleDbService, Task>> CollectAsyncDeleteOperations(IGrouping<ISimpleDbDomain, ISessionItem> domainSet)
        {
            var deleteBatches = domainSet.Where(i => i.State == SessionItemState.Delete).GroupsOf(25);
            return deleteBatches.Select(deleteBatch => (Func<ISimpleDbService, Task>)(async service => await service.BatchDeleteAttributesAsync(domainSet.Key.Name, deleteBatch.Cast<object>().ToArray()).ConfigureAwait(false)));
        }

        private static IEnumerable<Func<ISimpleDbService, Task>> CollectAsyncUpsertOperations(IGrouping<ISimpleDbDomain, ISessionItem> domainSet)
        {
            var putBatches = domainSet.Where(i => i.State == SessionItemState.Create || i.State == SessionItemState.Update).GroupsOf(25);
            return putBatches.Select(putBatch => (Func<ISimpleDbService, Task>) (async service => await service.BatchPutAttributesAsync(domainSet.Key.Name, putBatch.Cast<object>().ToArray()).ConfigureAwait(false)));
        }
    }
}