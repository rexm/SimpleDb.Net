using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionSimpleDbContext : ISimpleDbContext, IInternalContext
    {
        private readonly ISimpleDbService _service;
        private readonly ISession _session;
        private readonly ISimpleDbStatistics _statistics;

        ISimpleDbService IInternalContext.Service { get { return _service; } }

        ISession IInternalContext.Session { get { return _session; } }

        internal SessionSimpleDbContext(ISimpleDbService service)
        {
            _service = new StatisticsCollectorProxy(service);
            _statistics = (ISimpleDbStatistics)_service; //this feels dirty
            _session = new SimpleDbSession(_service);
        }

        public ISimpleDbDomainCollection Domains
        {
            get { return new SessionSimpleDbDomainCollection(this); }
        }

        public void SubmitChanges()
        {
            SubmitChangesAsync().Wait();
        }

        public async Task SubmitChangesAsync()
        {
            await _session.SubmitChangesAsync().ConfigureAwait(false);
        }

        public ISimpleDbStatistics Statistics
        {
            get { return _statistics; }
        }

        public void Dispose()
        {
        }
    }
}
