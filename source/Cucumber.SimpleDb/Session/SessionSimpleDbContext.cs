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

        ISimpleDbService IInternalContext.Service { get { return _service; } }

        ISession IInternalContext.Session { get { return _session; } }

        internal SessionSimpleDbContext(ISimpleDbService service)
        {
            _service = service;
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
