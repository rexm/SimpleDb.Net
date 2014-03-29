using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Async.Session
{
    internal sealed class SessionSimpleDbContext : ISimpleDbContext, IInternalContext
    {
        private readonly ISimpleDbService _service;
        private readonly ISession _session;

        internal SessionSimpleDbContext(ISimpleDbService service, ISession session)
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

        public ISimpleDbDomainCollection Domains
        {
            get { return new SessionSimpleDbDomainCollection(this); }
        }

        public async Task SubmitChangesAsync()
        {
            await _session.SubmitChangesAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
        }
    }
}