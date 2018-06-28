using System;
using System.Collections.Generic;
using System.Text;
using TejiLib;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TejiClient {

    public class Server {

        public Server(Socket s,string guid) {
            server = s;
            this.Guid = guid;
            AbandonedServer = false;

            this.ReceiveData();
        }

        Socket server;
        public string Guid { get; private set; }
        public string EndPoint {
            get {
                return server.RemoteEndPoint.ToString();
            }
        }
        public bool AbandonedServer { get; private set; }
        byte[] buffer;

        public event Action<string> NewMessage;

        private void OnNewMessage(string msg) {
            NewMessage?.Invoke(msg);
        }

        void ReceiveData() {
            if (AbandonedServer) return;
            Task.Run(() => {
                try {
                    buffer = new byte[4];
                    server.Receive(buffer, 0, 4, SocketFlags.None);
                    int msgLength = BitConverter.ToInt32(buffer, 0);
                    buffer = new byte[msgLength];

                    server.Receive(buffer, 0, msgLength, SocketFlags.None);
                    OnNewMessage(Information.UniversalEncoding.GetString(buffer));

                    this.ReceiveData();
                } catch (Exception) {
                    //abandon
                    AbandonedServer = true;
                }
            });
        }

        void SendDataCore(byte[] data) {
            if (AbandonedServer) return;
            Task.Run(() => {
                try {
                    var length = BitConverter.GetBytes(data.Length);
                    server.Send(length, 0, 4, SocketFlags.None);
                    server.Send(data, 0, data.Length, SocketFlags.None);
                } catch (Exception) {
                    //abandon
                    AbandonedServer = true;
                }
            });
        }

        public void Close() {
            if (AbandonedServer) return;
            AbandonedServer = true;
            try {
                server.Close();
            } catch (Exception) {
                //pass
            }
        }

        public void SendData(string str) {
            this.SendDataCore(Information.UniversalEncoding.GetBytes(str));
        }

    }
}
