using System;
using System.IO;

namespace CsLox
{
    public class LoxInterpreter
    {
        private readonly ConsoleLogger Logger;
        public LoxInterpreter()
        {
            Logger = new ConsoleLogger(LogLevel.Debug);
        }


        public bool RunFile(string path)
        {
            Logger.Debug($"Run file {path}...");
            var success = Run(File.ReadAllText(path));

            return success;
        }

        public bool RunPrompt()
        {
            bool isRunning = true;
            ConsoleCancelEventHandler onCtrlC = (_, __) => {
                isRunning = false;
                Logger.Info("[+] Exit. Bye!");
            };

            Console.CancelKeyPress += onCtrlC;

            Console.WriteLine(File.ReadAllText("info.txt"));
            while (isRunning)
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if (line != null)
                {
                    Run(line);
                }
            }

            Console.CancelKeyPress -= onCtrlC;

            return true;
        }

        public bool Run(string source)
        {
            var scanner = new LoxScanner(Logger);
            var tokens = scanner.ScanTokens(source);

            Logger.Debug($"TOKENS: \n  {string.Join("\n  ", tokens)}");

            if (!scanner.IsSuccess)
            {
                Logger.Info("Please fix syntax errors.");
                return false;
            }

            return true;
        }
    }
}

