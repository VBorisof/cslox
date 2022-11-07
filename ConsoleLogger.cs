using System;
using System.Linq;

namespace CsLox
{
    public static class ConsoleEx
    {
        public static void WriteLine(string input, ConsoleColor color)
        {
            var originalFg = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.WriteLine(input);

            Console.ForegroundColor = originalFg;
        }

        public static void Write(string input, ConsoleColor color)
        {
            var originalFg = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(input);

            Console.ForegroundColor = originalFg;
        }
    }

    public enum LogLevel
    {
        Error,
        Info,
        Debug,
    }

    public class ConsoleLogger
    {
        private readonly LogLevel _level;

        public ConsoleLogger(LogLevel level)
        {
            _level = level;
        }

        public void Error(int lineNumber, int? colNumber, string line, string message)
        {
            if (_level < LogLevel.Error) return;

            ConsoleEx.WriteLine($"Error:\n", ConsoleColor.Red);
            string marginColumn = $"    {lineNumber} | ";
            ConsoleEx.Write(marginColumn, ConsoleColor.Yellow);
            ConsoleEx.WriteLine(line, ConsoleColor.White);
            if (colNumber != null)
            {
                Console.Write(new string(' ', marginColumn.Length));
                ConsoleEx.WriteLine(
                    new string('~', colNumber.Value-1) + '^',
                    ConsoleColor.Yellow
                );
            }
            ConsoleEx.WriteLine($"    {message}\n", ConsoleColor.Red);
        }

        public void Info(string message)
        {
            if (_level < LogLevel.Info) return;

            ConsoleEx.WriteLine($"Info:\n", ConsoleColor.DarkYellow);
            ConsoleEx.WriteLine($"    {message}", ConsoleColor.Yellow);
        }


        public void Debug(string message)
        {
            if (_level < LogLevel.Debug) return;

            ConsoleEx.WriteLine($"[D]: {message}\n", ConsoleColor.Gray);
        }
    }
}

