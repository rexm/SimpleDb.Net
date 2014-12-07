using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cucumber.SimpleDb.Session
{
    internal interface ISessionAttribute
    {
        bool IsDirty { get; }
        bool IsDeleted { get; }
        ISessionItem Item { get; }
    }
}