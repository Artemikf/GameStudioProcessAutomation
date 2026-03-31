using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Patterns.Observer
{
    // Конкретный субъект - сервис уведомлений
    public class NotificationService : ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();
        private static NotificationService _instance;

        private NotificationService() { }

        public static NotificationService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NotificationService();
            }
            return _instance;
        }

        public void Attach(IObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                System.Diagnostics.Debug.WriteLine($"Observer '{observer.GetName()}' attached");
            }
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
            System.Diagnostics.Debug.WriteLine($"Observer '{observer.GetName()}' detached");
        }

        public void Notify(string entityType, string action, string entityName)
        {
            var message = $"[{DateTime.Now:HH:mm:ss}] {entityType} '{entityName}' was {action}";

            foreach (var observer in _observers.ToList())
            {
                observer.Update(message, entityType, action);
            }
        }
    }
}