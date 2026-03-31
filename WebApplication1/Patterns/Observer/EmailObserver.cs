using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Patterns.Observer
{
    // Наблюдатель для email уведомлений (администраторам)
    public class EmailObserver : IObserver
    {
        private string _name = "EmailNotifier";
        private List<string> _adminEmails = new List<string>();
        private bool _notifyOnlyAdminActions;

        public EmailObserver(bool notifyOnlyAdminActions = true)
        {
            _notifyOnlyAdminActions = notifyOnlyAdminActions;
            _adminEmails.Add("admin@company.com");
            _adminEmails.Add("manager@company.com");
        }

        public void Update(string message, string entityType, string action)
        {
            // В реальном проекте здесь был бы код отправки email
            // Для демонстрации просто логируем
            if (_notifyOnlyAdminActions && action == "deleted")
            {
                System.Diagnostics.Debug.WriteLine($"[EMAIL] Sending to {string.Join(", ", _adminEmails)}: {message}");
            }
            else if (!_notifyOnlyAdminActions)
            {
                System.Diagnostics.Debug.WriteLine($"[EMAIL] {message}");
            }
        }

        public string GetName() => _name;
    }
}