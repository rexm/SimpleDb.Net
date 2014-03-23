namespace Cucumber.SimpleDb.Session
{
    internal interface ISessionAttribute
    {
        bool IsDirty { get; }
    }
}