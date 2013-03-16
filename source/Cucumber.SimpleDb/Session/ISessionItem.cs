using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cucumber.SimpleDb.Session
{
    internal interface ISessionItem
    {
        IEnumerable<ISimpleDbAttribute> Attributes { get; }
        SessionItemState State { get; }
        ISimpleDbDomain Domain { get; }
        string Name { get; }
    }

    internal enum SessionItemState
    {
        Unchanged = 0,
        Update = 1,
        Create = 2,
        Delete = 4
    }
}
