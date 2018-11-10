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
            ConsoleAssistance.WriteLine($"[Network] Listening on port {General.serverConfig["ipv4Port"]} for ipv4 connection.");
            socket6.Listen(5);
            GetCaller(socket6);
            ConsoleAssistance.WriteLine($"[Network] Listening on port {General.serverConfig["ipv6Port"]} for ipv6 connection.");

            isListening = true;
        }

        public void StopListen() {
            if (!isListening) return;

            socket4.Close();
            ConsoleAssistance.WriteLine("[Network] Stop listening ipv4 connection.");
            socket6.Close();
            ConsoleAssistance.WriteLine("[Network] Stop listening ipv6 connection.");

            isListening = false;
        }

        //automatically stop listen followed by defined value
        public void OnConnectionCountChanged() {
            //seperate to passby dead lock

            int count = int.Parse(General.serverConfig["maxUser"]);
            bool test = true;
            lock (lockClientList) {
                test = clientList.Count >= count;
            }

            if (test) StopListen();
            else StartListen();
        }

        void GetCaller(Socket s) {
            Task.Run(() => {
                try {
                    Socket client = s.Accept();
                    var cache = new Client(client, System.Guid.NewGuid().ToString());
                    //add event handle
                    cache.TextMessage += this.TextMessageHandle;
                    cache.CommandMessage += this.CommandMessageHandle;
                    cache.RequestMessage += this.RequestMessageHandle;
                    cache.FileHeadMessage += this.FileHeadMessageHandle;
                    cache.FileBodyMessage += this.FileBodyMessagehandle;
                    cache.E2EMessage += this.E2EMessagehandle;
                    cache.RequestRemove += this.RequestRemoveHandle;
                    //add item
                    clientList.Add(cache);
                    ConsoleAssistance.WriteLine($"[Network] Accept {cache.EndPoint}'s connection and its Guid is {cache.Guid}.");

                    OnConnectionCountChanged();
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

        void TextMessageHandle(Client client, string room, string msg) {

        }

        void CommandMessageHandle(Client client,string command) {
            CommandProcessor.Process(client, command);
        }

        void RequestMessageHandle(Client client, string guid) {

        }

        void FileHeadMessageHandle(Client client, string guid, int section_count, int section_length, int last_section_length) {

        }

        void FileBodyMessagehandle(Client client,string guid, int index, byte[] data) {

        }

        void E2EMessagehandle(Client client, string to, byte[] data) {

        }

        void RequestRemoveHandle(Client client) {
            lock (lockClientList) {
                clientList.Remove(client);
            }
        }


        #endregion

    }

}
