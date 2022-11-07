using System;

namespace CsLox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length > 1)
            {
                Console.WriteLine("[!] Usage: cslox [script].");
                Environment.Exit(69);
            }
         
            var it = new LoxInterpreter();
            var success = true;
            if (args.Length == 1)
            {
                it.RunFile(args[0]);
            }
            else {
                it.RunPrompt();
            }

            if (!success)
            {
                Environment.Exit(65);
            }
        }
    }
}
