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

        public void ConnectServer(string ip, int port, bool isIpv6, string username, string password) {
            Task.Run(() => {
                try {
                    Socket con;
                    if (isIpv6) con = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                    else con = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    ConsoleAssistance.WriteLine($"[Network] Connecting {ip}:{port.ToString()}", ConsoleColor.Yellow);
                    con.Connect(ip, port);

                    var cache = new Server(con, System.Guid.NewGuid().ToString(), username, password);
                    cache.TextMessage += this.TextMessageHandle;
                    cache.ResponseMessage += this.ResponseMessageHandle;
                    cache.RequestMessage += this.RequestMessageHandle;
                    cache.FileHeadMessage += this.FileHeadMessageHandle;
                    cache.FileBodyMessage += this.FileBodyMessagehandle;
                    cache.E2EMessage += this.E2EMessagehandle;
                    cache.RequestRemove += this.RequestRemoveHandle;
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

        #endregion

        object lockServerList = new object();
        List<Server> serverList = new List<Server>();

        void TextMessageHandle(Server server, string room, string user, long time_stamp, string words) {

        }

        void ResponseMessageHandle(Server server, string words) {

        }

        void RequestMessageHandle(Server server, string guid) {

        }

        void BroadcastMessageHandle(Server server, string guid) {

        }

        void FileHeadMessageHandle(Server server, string guid, int section_count, int section_length, int last_section_length) {

        }

        void FileBodyMessagehandle(Server server, string guid, int index, byte[] data) {

        }

        void E2EMessagehandle(Server server, string from, byte[] data) {

        }

        void RequestRemoveHandle(Server server) {
            lock (lockServerList) {
                serverList.Remove(server);
            }
        }
    }
}
