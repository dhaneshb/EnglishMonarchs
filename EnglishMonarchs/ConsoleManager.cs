using System;

namespace EnglishMonarchs
{
    public interface IConsoleManager
    {
        void ErrorWriteLine(string value);
        void WriteLine(string value);
        string ReadLine();
        void Clear();
    }

    public abstract class ConsoleManagerBase : IConsoleManager
    {
        public abstract void ErrorWriteLine(string value);
        public abstract void Write(string value);
        public abstract void WriteLine(string value);
        public abstract ConsoleKeyInfo ReadKey();
        public abstract string ReadLine();
        public abstract void Clear();
    }

    public class ConsoleManager : ConsoleManagerBase
    {
        public override void ErrorWriteLine(string value)
        {
            Console.Error.WriteLine(value);
            var keyInfo = ReadKey();
            while (keyInfo.Key != ConsoleKey.Enter)
                keyInfo = ReadKey();
            Environment.Exit(0);
        }

        public override void Write(string value)
        {
            Console.Write(value);
        }

        public override void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public override ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        public override void Clear()
        {
            Console.Clear();
        }
    }
}
