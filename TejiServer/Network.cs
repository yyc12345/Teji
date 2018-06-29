using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TejiLib;
using System.Threading.Tasks;

namespace TejiServer {

    public class Network {

        public Network() {
            tdClientListCleaner = new Thread(ClientListCleaner);
            tdClientListCleaner.IsBackground = true;
            tdClientListCleaner.Start();
        }

        public void Close() {
            StopListen();
            //tdClientListCleaner.Abort();
            lock (lockClientList) {
                foreach (var item in clientList) {
                    item.Close();
                }
            }
        }

        #region listen

        Socket socket4;
        Socket socket6;

        bool isListening = false;

        public void StartListen() {
            if (isListening) return;

            socket4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            var endPoint4 = new IPEndPoint(IPAddress.Any, int.Parse(General.serverConfig["ipv4Port"]));
            var endPoint6 = new IPEndPoint(IPAddress.IPv6Any, int.Parse(General.serverConfig["ipv6Port"]));

            socket4.Bind(endPoint4);
            socket6.Bind(endPoint6);

            socket4.Listen(5);
            GetCaller(socket4);
            ConsoleAssistance.WriteLine($"[Socket] Listening on port {General.serverConfig["ipv4Port"]} for ipv4 connection.");
            socket6.Listen(5);
            GetCaller(socket6);
            ConsoleAssistance.WriteLine($"[Socket] Listening on port {General.serverConfig["ipv6Port"]} for ipv6 connection.");

            isListening = true;
        }

        public void StopListen() {
            if (!isListening) return;

            socket4.Close();
            ConsoleAssistance.WriteLine("[Socket] Stop listening ipv4 connection.");
            socket6.Close();
            ConsoleAssistance.WriteLine("[Socket] Stop listening ipv6 connection.");

            isListening = false;
        }

        void GetCaller(Socket s) {
            Task.Run(() => {
                try {
                    Socket client = s.Accept();
                    var cache = new Client(client, System.Guid.NewGuid().ToString());
                    cache.NewMessage += NewMessage;
                    clientList.Add(cache);
                    ConsoleAssistance.WriteLine($"[Socket] Accept {cache.EndPoint}'s connection and its Guid is {cache.Guid}.");
                } catch (Exception) {
                    //jump
                    return;
                }

                //accept next
                this.GetCaller(s);
            });
        }

        #endregion

        #region socket_event

        object lockClientList = new object();
        List<Client> clientList = new List<Client>();

        void NewMessage(string str) {
            lock (lockClientList) {
                foreach (var item in clientList) {
                    item.SendData(str);
                }
            }
        }

        Thread tdClientListCleaner;
        void ClientListCleaner() {
            Thread.Sleep(1000 * 60 * 1);

            lock (lockClientList) {
                foreach (var item in clientList) {
                    if (item.AbandonedClient) clientList.Remove(item);
                }
            }

        }


        #endregion

    }

}
