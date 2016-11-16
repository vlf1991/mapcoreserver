using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace DemoHeatmap
{
    public static class Debug
    {
        class raise
        {
            public ConsoleColor color;
            public string prefix;
            public int severity;

            public raise(ConsoleColor Color, string Prefix = "", int Severity = 0)
            {
                color = Color;
                prefix = Prefix;
                severity = Severity;
            }
        }

        public static void Log(string text, params object[] a)
        {
            print(text, new raise(ConsoleColor.Gray), a);
        }

        public static void Info(string text, params object[] a)
        {
            print(text, new raise(ConsoleColor.DarkGray), a);
        }

        public static void Warn(string text, params object[] a)
        {
            print(text, new raise(ConsoleColor.Yellow, "WARN", 1), a);
        }

        public static void Error(string text, params object[] a)
        {
            print(text, new raise(ConsoleColor.DarkRed, "ERROR", 2), a);
        }

        public static void Success(string text, params object[] a)
        {
            print(text, new raise(ConsoleColor.Green, "SUCCESS", 2), a);
        }

        private static void print(string text, raise context, params object[] a)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(DateTime.UtcNow + " ");

            Console.ForegroundColor = context.color;

            Console.WriteLine(context.prefix + ": " + String.Format(text, a));

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
