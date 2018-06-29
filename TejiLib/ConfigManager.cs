using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TejiLib {
    public class ConfigManager {

        public ConfigManager(bool isServer) {

            if (!File.Exists(Information.WorkPath.Enter("teji.cfg").Path())) {
                ConsoleAssistance.WriteLine("[Config] Generate default config...");
                Generate(isServer);
                return;
            }

            read:
            var file = new StreamReader(Information.WorkPath.Enter("teji.cfg").Path(), Information.UniversalEncoding);
            var str = file.ReadToEnd();
            file.Close();

            try {
                config = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
                ConsoleAssistance.WriteLine("[Config] Read config successfully.");
            } catch (Exception) {
                //wrong format
                Generate(isServer);
                goto read;
            }
        }

        object lock_config = new Object();
        Dictionary<string, string> config = new Dictionary<string, string>();

        public void Generate(bool isServer) {
            if (isServer) {
                config.Add("ipv4Port", "8181");
                config.Add("ipv6Port", "6161");
                config.Add("maxUser", "10");
            } else {
                config.Add("PlaceHolder", "");
            }

            //var cache = new ServerConfigItem() {
            //    userDatabasePath = Information.WorkPath.Enter("user.db").Path(),
            //    roomDatabasePath = Information.WorkPath.Enter("room.db").Path(),
            //    banDatabasePath = Information.WorkPath.Enter("ban.db").Path(),
            //    emotionDatabasePath = Information.WorkPath.Enter("emotion.db").Path(),
            //    ipv4Port = "8686",
            //    ipv6Port = "6161",
            //    maxUser = "10"
            //};

            this.Save();
        }

        public void Save() {
            lock (lock_config) {
                var file = new StreamWriter(Information.WorkPath.Enter("teji.cfg").Path(), false, Information.UniversalEncoding);
                file.Write(JsonConvert.SerializeObject(config));
                file.Close();
                ConsoleAssistance.WriteLine("[Config] Save config successfully.");
            }
        }

        public string this[string key] {
            get {
                lock (lock_config) {
                    return config[key];
                }
            }
        }

    }
}
