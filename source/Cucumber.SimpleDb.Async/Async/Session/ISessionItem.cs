using System.Collections.Generic;

namespace Cucumber.SimpleDb.Async.Session
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