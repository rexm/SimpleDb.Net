using Cucumber.SimpleDb.Session;

namespace Cucumber.SimpleDb
{
    internal interface IInternalContext
    {
        ISimpleDbService Service { get; }
        ISession Session { get; }
    }
}