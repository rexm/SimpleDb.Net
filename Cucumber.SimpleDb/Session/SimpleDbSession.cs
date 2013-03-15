using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Utilities;

namespace Cucumber.SimpleDb.Session
{
    internal class SimpleDbSession : ISession
    {
        private readonly HashSet<ISessionItem> _trackedItems;
        private readonly ISimpleDbService _service;

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
            try
            {
                foreach (var operation in CollectOperations())
                {
                    operation(_service);
                }
            }
            catch
            {
                throw;
            }
        }

        private IEnumerable<Action<ISimpleDbService>> CollectOperations()
        {
            var domainSets = _trackedItems.GroupBy(item => item.Domain, (x, y) => x.Name == y.Name);
            foreach(var action in domainSets.SelectMany(domainSet => CollectDeleteOperations(domainSet)))
            {
                yield return action;
            }
            foreach (var action in domainSets.SelectMany(domainSet => CollectUpsertOperations(domainSet)))
            {
                yield return action;
            }
        }

        private IEnumerable<Action<ISimpleDbService>> CollectDeleteOperations(IGrouping<ISimpleDbDomain, ISessionItem> domainSet)
        {
            var deleteBatches = domainSet.Where(i => i.State == SessionItemState.Delete).GroupsOf(25);
            foreach (var deleteBatch in deleteBatches)
            {
                yield return service => service.BatchDeleteAttributes(domainSet.Key.Name, deleteBatch);
            }
        }

        private IEnumerable<Action<ISimpleDbService>> CollectUpsertOperations(IGrouping<ISimpleDbDomain, ISessionItem> domainSet)
        {
            var putBatches = domainSet.Where(i => i.State == SessionItemState.Create || i.State == SessionItemState.Update).GroupsOf(25);
            foreach (var putBatch in putBatches)
            {
                yield return service => service.BatchPutAttributes(domainSet.Key.Name, putBatch);
            }
        }
    }
}
