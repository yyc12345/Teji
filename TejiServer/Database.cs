using System;
using System.Collections.Generic;
using System.Text;
using TejiLib;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
//using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TejiServer {

    public class Database {

        public void Open() {
            ConsoleAssistance.WriteLine("[Database] Reading database...");
            CoreDbContext = new ServerDataContext();
            ConsoleAssistance.WriteLine("[Database] Making sure all database is fine...");
            CoreDbContext.Database.EnsureCreated();

            ConsoleAssistance.WriteLine("[Database] Read database successfully.");

            //start a thread for saving database
            databaseSavingTd = new Thread(() => {
                Thread.Sleep(1000 * 60 * 10);
                lock (lockCoreDbContext) {
                    CoreDbContext.SaveChanges();
                }
                ConsoleAssistance.WriteLine("[Database] Save database successfully.");
            });
            databaseSavingTd.IsBackground = true;
            databaseSavingTd.Start();
        }

        public void Close() {
            try {
                databaseSavingTd.Abort();
            } catch (Exception) {
                //pass
            }
            lock (lockCoreDbContext) {
                CoreDbContext.SaveChanges();
            }
            ConsoleAssistance.WriteLine("[Database] Save database successfully.");
            CoreDbContext.Dispose();
        }

        ServerDataContext CoreDbContext;
        object lockCoreDbContext = new object();
        Thread databaseSavingTd;

        //todo:finish database operation

        #region user

        public (bool isOK, string message, byte[] salt1, byte[] salt2) GetSalt(string user) {
            lock (lockCoreDbContext) {
                var res = (from item in CoreDbContext.user
                           where item.name == user
                           select item).ToList();
                if (!res.Any()) return (false, "No matched name", default(byte[]), default(byte[]));
                return (true, "", Convert.FromBase64String(res[0].salt1), Convert.FromBase64String(res[0].salt2));
            }
        }

        #endregion

        #region room


        #endregion

        #region ban


        #endregion

        #region emotion

        #endregion

    }


    public class ServerDataContext : DbContext {
        public DbSet<UserDatabaseItem> user { get; set; }
        public DbSet<RoomDatabaseItem> room { get; set; }
        public DbSet<BanDatabaseItem> ban { get; set; }
        public DbSet<EmotionDatabaseItem> emotion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source = {Information.WorkPath.Enter("server.db").Path()};");
        }
    }

    [Table("user")]
    public class UserDatabaseItem {
        [Key]
        public string name { get; set; }

        public string nickname { get; set; }
        public bool isAdmin { get; set; }
        public string avatarGuid { get; set; }
        public string salt1 { get; set; }
        public string salt2 { get; set; }
        public string saltHash { get; set; }
    }

    [Table("room")]
    public class RoomDatabaseItem {
        [Key]
        public string name { get; set; }

        public int permission { get; set; }
        public string password { get; set; }
        public string host { get; set; }
        public string users { get; set; }
    }

    [Table("ban")]
    public class BanDatabaseItem {
        [Key]
        public string value { get; set; }

        public string type { get; set; }
    }

    [Table("emotion")]
    public class EmotionDatabaseItem {
        [Key]
        public string name { get; set; }

        public string emotionGuid { get; set; }
    }

}
