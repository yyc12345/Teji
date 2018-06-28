using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TejiLib;

namespace TejiServer {

    public class Client {

        public Client(Socket s, string guid) {
            client = s;
            this.Guid = guid;
            AbandonedClient = false;

            this.ReceiveData();
        }

        Socket client;
        public string Guid { get; private set; }
        public string EndPoint {
            get {
                return client.RemoteEndPoint.ToString();
            }
        }
        public bool AbandonedClient { get; private set; }
        byte[] buffer;

        public event Action<string> NewMessage;

        private void OnNewMessage(string msg) {
            NewMessage?.Invoke(msg);
        }

        void ReceiveData() {
            if (AbandonedClient) return;
            Task.Run(() => {
                try {
                    buffer = new byte[4];
                    client.Receive(buffer, 0, 4, SocketFlags.None);
                    int msgLength = BitConverter.ToInt32(buffer, 0);
                    buffer = new byte[msgLength];

                    client.Receive(buffer, 0, msgLength, SocketFlags.None);
                    OnNewMessage(Information.UniversalEncoding.GetString(buffer));

                    this.ReceiveData();
                } catch (Exception) {
                    //abandon
                    AbandonedClient = true;
                }
            });
        }

        void SendDataCore(byte[] data) {
            if (AbandonedClient) return;
            Task.Run(() => {
                try {
                    var length = BitConverter.GetBytes(data.Length);
                    client.Send(length, 0, 4, SocketFlags.None);
                    client.Send(data, 0, data.Length, SocketFlags.None);
                } catch (Exception) {
                    //abandon
                    AbandonedClient = true;
                }
            });
        }

        public void Close() {
            if (AbandonedClient) return;
            AbandonedClient = true;
            try {
                client.Close();
            } catch (Exception) {
                //pass
            }
        }

        public void SendData(string str) {
            this.SendDataCore(Information.UniversalEncoding.GetBytes(str));
        }

    }
}
