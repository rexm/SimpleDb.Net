using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Transport;
using Cucumber.SimpleDb.Session;

namespace Cucumber.SimpleDb
{
    internal interface IInternalContext
    {
        ISimpleDbService Service { get; }
        ISession Session { get; }
    }
}
