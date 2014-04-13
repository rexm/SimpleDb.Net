namespace Cucumber.SimpleDb.Async
{
    internal interface IInternalContext
    {
        ISimpleDbService Service { get; }
        ISession Session { get; }
    }
}