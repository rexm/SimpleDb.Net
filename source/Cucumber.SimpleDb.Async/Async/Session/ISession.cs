namespace Cucumber.SimpleDb.Async.Session
{
    internal interface ISession
    {
        void Attach(ISessionItem item);
        void Detatch(ISessionItem item);
        void SubmitChanges();
    }
}