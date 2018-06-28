using System;
using TejiClient;
using TejiLib;

namespace TejiConsole {
    class Program {
        static void Main(string[] args) {

            ConsoleAssistance.WriteLine("Teji client " + Information.Version, ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("A self-host IM.");

            ConsoleAssistance.WriteLine("Init Network...");
            var cache = new Network();
            cache.ServerMessage += (str) => {
                ConsoleAssistance.WriteLine(str);
            };
            cache.ConnectServer("192.168.1.102", 8181, false);
            //cache.ConnectServer("fe80:0:5c1f:28d1:53ba:7fcd", 6161, true);

            Console.ReadKey();
            cache.SendMessage("test", "???");
            Console.ReadKey();
            cache.Close();
            Console.ReadKey();


        }
    }
}
