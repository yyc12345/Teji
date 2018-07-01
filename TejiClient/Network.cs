using System;
using System.Collections.Generic;
using System.Text;
using TejiLib;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace TejiClient {
    public class Network {

        public Network() {
            tdServerListCleaner = new Thread(ServerListCleaner);
            tdServerListCleaner.IsBackground = true;
            tdServerListCleaner.Start();
        }

        public void Close() {
            //tdServerListCleaner.Abort();
            lock (lockServerList) {
                foreach (var item in serverList) {
                    item.Close();
                }
            }
        }

        #region export

        public event Action<string> ServerMessage;

        private void OnServerMessage(string msg) {
            ServerMessage?.Invoke(msg);
        }

        public void ConnectServer(string ip, int port, bool isIpv6) {
            Task.Run(() => {
                try {
                    Socket con;
                    if (isIpv6) con = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                    else con = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    ConsoleAssistance.WriteLine($"[Network] Connecting {ip}:{port.ToString()}", ConsoleColor.Yellow);
                    con.Connect(ip, port);

                    var cache = new Server(con, System.Guid.NewGuid().ToString());
                    cache.NewMessage += NewMessage;
                    ConsoleAssistance.WriteLine($"[Network] Connect {ip}:{port.ToString()} successfully and its Guid is {cache.Guid}.");
                    serverList.Add(cache);
                } catch (Exception) {
                    //abandon
                    ConsoleAssistance.WriteLine($"[Network] Fail to connect {ip}:{port.ToString()}.", ConsoleColor.Red);
                }
            });
        }

        public void DisconnectServer(string guid) {
            Task.Run(() => {
                lock (lockServerList) {
                    foreach (var item in serverList) {
                        if (item.Guid == guid) item.Close();
                    }
                }
            });
        }

        public void SendMessage(string words,string guid) {
            //todo: test mode
            Task.Run(() => {
                lock (lockServerList) {
                    foreach (var item in serverList) {
                        item.SendData(words);
                    }
                }
            });
        }

        #endregion

        object lockServerList = new object();
        List<Server> serverList = new List<Server>();

        void NewMessage(string str) {
            OnServerMessage(str);
        }

        Thread tdServerListCleaner;
        void ServerListCleaner() {
            Thread.Sleep(1000 * 60 * 1);

            lock (lockServerList) {
                foreach (var item in serverList) {
                    if (item.AbandonedServer) serverList.Remove(item);
                }
            }

        }

    }
}
