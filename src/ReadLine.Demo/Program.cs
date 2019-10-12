using System;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("ReadLine Library Demo");
            Console.WriteLine("---------------------");
            Console.WriteLine();

            string[] history = new string[] { "ls -a", "dotnet run", "git init" };
            ReadLine.AddHistory(history);

            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

            string input = ReadLine.Read("(prompt)> ");
            Console.WriteLine(input);

            input = ReadLine.ReadPassword("Enter Password> ");
            Console.WriteLine(input);

            System.Threading.Tasks.Task.Run(() => {
                var a = ReadLine.Read("From Task >");
                Console.WriteLine(a);
            });
            System.Threading.Thread.Sleep(1000);
            ReadLine.Send(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));

            System.Threading.Tasks.Task.Run(() => {
                var a = ReadLine.Read("From Task >");
                Console.WriteLine(a);
            });
            System.Threading.Thread.Sleep(1000);
            ReadLine.Send("This is a slow string being typed\n", 500);
            System.Threading.Tasks.Task.Run(() => {
                var a = ReadLine.Read("From Task >");
                Console.WriteLine(a);
            });
            System.Threading.Thread.Sleep(1000);
            ReadLine.Send(ConsoleKey.UpArrow);
            //ReadLine.Send();

            Console.ReadKey(true);
        }
    }

    class AutoCompletionHandler : IAutoCompleteHandler
    {
        public char[] Separators { get; set; } = new char[] { ' ', '.', '/', '\\', ':' };
        public string[] GetSuggestions(string text, int index)
        {
            if (text.StartsWith("git "))
                return new string[] { "init", "clone", "pull", "push" };
            else
                return null;
        }
    }
}
