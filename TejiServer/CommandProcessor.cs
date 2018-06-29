using System;
using System.Collections.Generic;
using System.Text;
using TejiLib;

namespace TejiServer {
    public class CommandProcessor {

        public static void Process(string command) {
            var cache = CommandSplitter.SplitCommand(command);

            if (cache.Count == 0) {
                ConsoleAssistance.WriteLine("Error command", ConsoleColor.Red);
                return;
            }

            switch (cache[0]) {
                //todo:finish command
                default:
                    ConsoleAssistance.WriteLine("No such command.", ConsoleColor.Red);
                    break;
            }

        }

    }
}
