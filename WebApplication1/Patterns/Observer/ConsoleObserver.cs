using System;

namespace WebApplication1.Patterns.Observer
{
    // Наблюдатель для вывода в консоль
    public class ConsoleObserver : IObserver
    {
        private string _name = "ConsoleLogger";

        public void Update(string message, string entityType, string action)
        {
            Console.WriteLine($"[CONSOLE] {message}");

            // Разные цвета для разных действий
            if (action == "created")
                Console.ForegroundColor = ConsoleColor.Green;
            else if (action == "updated")
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (action == "deleted")
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"  -> {message}");
            Console.ResetColor();
        }

        public string GetName() => _name;
    }
}