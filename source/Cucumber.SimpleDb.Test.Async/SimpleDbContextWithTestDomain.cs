using System;
using System.Threading.Tasks;
using Cucumber.SimpleDb.Async;
using Cucumber.SimpleDb.Async.Session;

namespace Cucumber.SimpleDb.Test.Async
{
    internal class SimpleDbContextWithTestDomain : ISimpleDbContext, IInternalContext
    {
        private readonly ISimpleDbService _service;
        private readonly ISession _session;

        internal SimpleDbContextWithTestDomain(ISimpleDbService service, ISession session)
        {
            _service = service;
            _session = session;
        }

        ISimpleDbService IInternalContext.Service
        {
            get { return _service; }
        }

        ISession IInternalContext.Session
        {
            get { return _session; }
        }

        public Task<ISimpleDbDomainCollection> GetDomainsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ISimpleDbDomain> GetDomainAsync(string name)
        {
            return Task.FromResult<ISimpleDbDomain>(new ProxySimpleDbDomain(name, null, this));
        }

        public Task SubmitChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}