using System;
using TejiClient;
using TejiLib;

namespace TejiConsole {
    class Program {
        static void Main(string[] args) {

            ConsoleAssistance.WriteLine("Teji client " + Information.Version, ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("A self-host IM.");

            General.IsInputing = false;

            ConsoleAssistance.WriteLine("Init Config...");
            General.clientConfig = new ConfigManager(false);

            ConsoleAssistance.WriteLine("Init Network...");
            General.clientNetwork = new Network();
            General.clientNetwork.ServerMessage += (str) => {
                ConsoleAssistance.WriteLine(str);
            };
            //General.clientNetwork.ConnectServer("192.168.1.102", 8181, false);
            ////cache.ConnectServer("fe80:0:5c1f:28d1:53ba:7fcd", 6161, true);

            //Console.ReadKey();
            //General.clientNetwork.SendMessage("test", "???");
            //Console.ReadKey();
            //General.clientNetwork.Close();
            //Console.ReadKey();

            //circle
            string command = "";
            while (true) {
                var result = Console.ReadKey(true);
                if (result.Key == ConsoleKey.Tab) {
                    General.IsInputing = true;
                    command = "";

                    //todo:finish display
                    ConsoleAssistance.Write("TejiConsole", ConsoleColor.Green);
                    ConsoleAssistance.Write("@???", ConsoleColor.Green);
                    ConsoleAssistance.Write(": ", ConsoleColor.Yellow);
                    ConsoleAssistance.Write("/", ConsoleColor.Magenta);
                    ConsoleAssistance.Write(" >", ConsoleColor.Yellow);


                    command = Console.ReadLine();
                    //todo:process command
                    if (command == "exit") break;
                    else CommandProcessor.Process(command);
                    General.IsInputing = false;
                }
            }

            //close
            General.clientNetwork.Close();
            General.clientConfig.Save();

        }
    }
}
