using System;
using TejiLib;

namespace TejiServer {
    class Program {
        static void Main(string[] args) {

            ConsoleAssistance.WriteLine("Teji server " + Information.Version, ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("A self-host IM.");

            ConsoleAssistance.WriteLine("Init Network...");
            var cache = new Network();
            cache.StartListen();

            Console.ReadKey();
            cache.Close();
            Console.ReadKey();
        }
    }
}
