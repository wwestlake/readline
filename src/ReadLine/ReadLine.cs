using Internal.ReadLine;
using Internal.ReadLine.Abstractions;
using System.Collections.Generic;
using System.Threading;

namespace System
{
    public static class ReadLine
    {
        private static List<string> _history;

        static ReadLine()
        {
            _history = new List<string>();
        }

        public static void AddHistory(params string[] text) => _history.AddRange(text);
        public static List<string> GetHistory() => _history;
        public static void ClearHistory() => _history = new List<string>();
        public static bool HistoryEnabled { get; set; }
        public static IAutoCompleteHandler AutoCompletionHandler { private get; set; }

        public static string Read(string prompt = "", string @default = "")
        {
            Console.Write(prompt);
            KeyHandler keyHandler = new KeyHandler(new Console2(), _history, AutoCompletionHandler);
            string text = GetText(keyHandler);

            if (String.IsNullOrWhiteSpace(text) && !String.IsNullOrWhiteSpace(@default))
            {
                text = @default;
            }
            else
            {
                if (HistoryEnabled)
                    _history.Add(text);
            }

            return text;
        }

        public static string ReadPassword(string prompt = "")
        {
            Console.Write(prompt);
            KeyHandler keyHandler = new KeyHandler(new Console2() { PasswordMode = true }, null, null);
            return GetText(keyHandler);
        }

        private static object locker = new object();
        private static Queue<ConsoleKeyInfo> _queue = new Queue<ConsoleKeyInfo>();

        public static void Send(ConsoleKeyInfo key)
        {
            lock (locker)
            {
                _queue.Enqueue(key);
            }
        }

        public static void Send(ConsoleKey key)
        {
            var ch = (char)key;
            Send(new ConsoleKeyInfo(ch, key, false, false, false));
        }

        public static void Send(char ch) => Send((ConsoleKey)ch);

        public static void Send(string str, int delay = 0)
        {
            foreach (var ch in str)
            {
                Send(ch);
                if (delay > 0) Thread.Sleep(delay);
            }
        }

        public static void Send()
        {
            Send(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));
        }

        private static ConsoleKeyInfo TimedReadKey(bool intercept = false)
        {
            while (true)
            {
                lock (locker)
                {
                    if (_queue.Count > 0) return _queue.Dequeue();
                }
                if (Console.KeyAvailable) return Console.ReadKey(intercept);
                Thread.Sleep(10);
            }
        }


        private static string GetText(KeyHandler keyHandler)
        {
            ConsoleKeyInfo keyInfo = TimedReadKey(true);
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                keyHandler.Handle(keyInfo);
                keyInfo = TimedReadKey(true);
            }

            Console.WriteLine();
            return keyHandler.Text;
        }
    }
}
