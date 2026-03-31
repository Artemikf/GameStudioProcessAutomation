namespace WebApplication1.Patterns.Observer;

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify(string entityType, string action, string entityName);
}
