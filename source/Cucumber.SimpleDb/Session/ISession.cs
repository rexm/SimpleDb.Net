using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Transport;

namespace Cucumber.SimpleDb.Session
{
    internal interface ISession
    {
        void Attach(ISessionItem item);
        void Detatch(ISessionItem item);
        void SubmitChanges();
    }
}
