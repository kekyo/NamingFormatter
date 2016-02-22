using System;
using CenterCLR;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var formatted = Named.Format(
                "ABC{aaa:yyyy/MM/dd hh:mm:ss.fff}XYZ",
                Named.Pair("aaa", DateTime.Now));
            Console.WriteLine(formatted);
        }
    }
}
