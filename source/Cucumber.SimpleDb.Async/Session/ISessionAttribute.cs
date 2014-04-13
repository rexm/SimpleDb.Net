namespace Cucumber.SimpleDb.Async.Session
{
    internal interface ISessionAttribute
    {
        bool IsDirty { get; }
    }
}