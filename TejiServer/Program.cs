using System;
using TejiLib;

namespace TejiServer {
    class Program {
        static void Main(string[] args) {

            ConsoleAssistance.WriteLine("Teji server " + Information.Version, ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("A self-host IM.");

            ConsoleAssistance.WriteLine("Init Config...");
            General.serverConfig = new ConfigManager(true);

            ConsoleAssistance.WriteLine("Init Database...");
            General.serverDatabase = new Database();
            General.serverDatabase.Open();

            ConsoleAssistance.WriteLine("Init File Pool...");
            General.FilePoolManager = new FilePool();

            ConsoleAssistance.WriteLine("Init Network...");
            General.serverNetwork = new Network();
            General.serverNetwork.StartListen();

            //circle
            string command = "";
            while (true) {
                var result = Console.ReadKey(true);
                if (result.Key == ConsoleKey.Tab) {
                    General.IsInputing = true;
                    command = "";

                    //todo:finish display
                    ConsoleAssistance.Write("TejiServer", ConsoleColor.Green);
                    ConsoleAssistance.Write(">", ConsoleColor.Yellow);

                    command = Console.ReadLine();
                    //todo:process command
                    if (command == "exit") break;
                    else CommandProcessor.Process(null, command);
                    General.IsInputing = false;
                }
            }

            //close
            General.serverNetwork.Close();
            General.serverDatabase.Close();
            General.serverConfig.Save();
        }
    }
}
