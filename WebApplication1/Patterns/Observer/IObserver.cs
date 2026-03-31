namespace WebApplication1.Patterns.Observer;

public interface IObserver
{
    void Update(string message, string entityType, string action);
    string GetName();
}
