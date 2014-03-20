using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cucumber.SimpleDb.Session
{
    internal class SessionSimpleDbContext : ISimpleDbContext, IInternalContext
    {
        private readonly ISimpleDbService _service;
        private readonly ISession _session;
        private readonly bool _useConsistency;

        ISimpleDbService IInternalContext.Service { get { return _service; } }

        ISession IInternalContext.Session { get { return _session; } }

        public bool UseConsistency
        {
            get { return _useConsistency; }
        }

        internal SessionSimpleDbContext(ISimpleDbService service, bool useConsistency)
        {
            _service = service;
            _useConsistency = useConsistency;
            _session = new SimpleDbSession(_service);
        }

        public ISimpleDbDomainCollection Domains
        {
            get { return new SessionSimpleDbDomainCollection(this); }
        }

        public void SubmitChanges()
        {
            _session.SubmitChanges();
        }

        public void Dispose()
        {
        }
    }
}
