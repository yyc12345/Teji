using System;
using System.Collections.Generic;
using System.Text;
using TejiLib;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace TejiServer {

    public class Database {

        public Database() {
            if (CheckDatabase()) Open();
            else {
                Generate();
                Save();
            }

            //start a thread for saving database
            var td = new Thread(() => {
                Thread.Sleep(1000 * 60 * 10);
                Save();
            });
            td.IsBackground = true;
            td.Start();
        }

        bool CheckDatabase() {
            if (!System.IO.File.Exists(Information.WorkPath.Enter("user.dat").Path())) return false;
            if (!System.IO.File.Exists(Information.WorkPath.Enter("room.dat").Path())) return false;
            if (!System.IO.File.Exists(Information.WorkPath.Enter("ban.dat").Path())) return false;
            if (!System.IO.File.Exists(Information.WorkPath.Enter("emotion.dat").Path())) return false;
            return true;
        }

        void Open() {
            ConsoleAssistance.WriteLine("[Database] Reading all database...");

            var file = new StreamReader(Information.WorkPath.Enter("user.dat").Path(), Information.UniversalEncoding);
            lock (lockUserDatabase) {
                userDatabase = JsonConvert.DeserializeObject<Dictionary<string, UserDatabaseItem>>(file.ReadToEnd());
            }
            file.Close();
            file.Dispose();

            file = new StreamReader(Information.WorkPath.Enter("room.dat").Path(), Information.UniversalEncoding);
            lock (lockUserDatabase) {
                roomDatabase = JsonConvert.DeserializeObject<Dictionary<string, RoomDatabaseItem>>(file.ReadToEnd());
            }
            file.Close();
            file.Dispose();

            file = new StreamReader(Information.WorkPath.Enter("ban.dat").Path(), Information.UniversalEncoding);
            lock (lockUserDatabase) {
                banDatabase = JsonConvert.DeserializeObject<Dictionary<string, BanDatabaseItem>>(file.ReadToEnd());
            }
            file.Close();
            file.Dispose();

            file = new StreamReader(Information.WorkPath.Enter("emotion.dat").Path(), Information.UniversalEncoding);
            lock (lockUserDatabase) {
                emotionDatabase = JsonConvert.DeserializeObject<Dictionary<string, EmotionDatabaseItem>>(file.ReadToEnd());
            }
            file.Close();
            file.Dispose();

            ConsoleAssistance.WriteLine("[Database] Read all database successfully.");
        }

        void Generate() {
            ConsoleAssistance.WriteLine("[Database] Generating all database...");
            userDatabase.Clear();
            roomDatabase.Clear();
            banDatabase.Clear();
            emotionDatabase.Clear();
            ConsoleAssistance.WriteLine("[Database] Generate all database successfully.");
        }

        public void Save() {
            ConsoleAssistance.WriteLine("[Database] Saving all database...");

            var file = new StreamWriter(Information.WorkPath.Enter("user.dat").Path(), false, Information.UniversalEncoding);
            lock (lockUserDatabase) {
                file.Write(JsonConvert.SerializeObject(userDatabase));
            }
            file.Close();
            file.Dispose();

            file = new StreamWriter(Information.WorkPath.Enter("room.dat").Path(), false, Information.UniversalEncoding);
            lock (lockUserDatabase) {
                file.Write(JsonConvert.SerializeObject(roomDatabase));
            }
            file.Close();
            file.Dispose();

            file = new StreamWriter(Information.WorkPath.Enter("ban.dat").Path(), false, Information.UniversalEncoding);
            lock (lockUserDatabase) {
                file.Write(JsonConvert.SerializeObject(banDatabase));
            }
            file.Close();
            file.Dispose();

            file = new StreamWriter(Information.WorkPath.Enter("emotion.dat").Path(), false, Information.UniversalEncoding);
            lock (lockUserDatabase) {
                file.Write(JsonConvert.SerializeObject(emotionDatabase));
            }
            file.Close();
            file.Dispose();

            ConsoleAssistance.WriteLine("[Database] Save all database successfully.");
        }

        //todo:finish database operation
        #region user

        object lockUserDatabase = new Object();
        Dictionary<string, UserDatabaseItem> userDatabase = new Dictionary<string, UserDatabaseItem>();


        #endregion

        #region room

        object lockRoomDatabase = new Object();
        Dictionary<string, RoomDatabaseItem> roomDatabase = new Dictionary<string, RoomDatabaseItem>();


        #endregion

        #region ban

        object lockBanDatabase = new Object();
        Dictionary<string, BanDatabaseItem> banDatabase = new Dictionary<string, BanDatabaseItem>();


        #endregion

        #region emotion

        object lockEmotionDatabase = new Object();
        Dictionary<string, EmotionDatabaseItem> emotionDatabase = new Dictionary<string, EmotionDatabaseItem>();


        #endregion

    }

    public struct UserDatabaseItem {
        public string name;
        public string nickname;
        public bool isAdmin;
        public string avatarGuid;
        public string salt1;
        public string salt2;
        public string saltHash;
    }

    public struct RoomDatabaseItem {
        public string name;
        public string type;
        public string password;
        public string host;
        public string users;
    }

    public struct BanDatabaseItem {
        public string value;
        public string type;
    }

    public struct EmotionDatabaseItem {
        public string name;
        public string emotionGuid;
    }
}
