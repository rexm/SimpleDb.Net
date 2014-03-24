using Cucumber.SimpleDb.Async.Session;

namespace Cucumber.SimpleDb.Async
{
    internal interface IInternalContext
    {
        ISimpleDbService Service { get; }
        ISession Session { get; }
    }
}