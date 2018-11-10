using System;
using System.Collections.Generic;
using System.Text;
using TejiLib;

namespace TejiServer {
    public class CommandProcessor {

        public static string Process(Client invoker, string command) {
            var cache = CommandSplitter.SplitCommand(command);

            if (cache.Count == 0) {
                return "Error command";
            }

            switch (cache[0]) {
                //todo:finish command
                default:
                    return "No such command.";
            }

        }

    }
}
