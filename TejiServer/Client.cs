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
            NewPlainMessage += this.ReceiveData;

            client = s;
            this.Guid = guid;
            this.UserName = "";
        }

        Socket client;
        public string Guid { get; private set; }
        public string UserName { get; private set; }
        public string EndPoint {
            get {
                return client.RemoteEndPoint.ToString();
            }
        }

        public void Close() {
            client.Close();
            RequestRemove.Invoke(this);
        }

        #region secure shell

        void SecureShellReceiveData() {
            try {
                byte[] length_buffer = new byte[4];
                int length = 0;
                byte[] data_buffer;
                while (true) {
                    client.Receive(length_buffer, 0, 4, SocketFlags.None);
                    length = BitConverter.ToInt32(length_buffer, 0);
                    data_buffer = new byte[length];
                    client.Receive(data_buffer, 0, 4, SocketFlags.None);
                    this.NewPlainMessage?.Invoke(data_buffer);
                }
            } catch (Exception) {
                //error
                this.Close();
            }
        }

        void SecureShellSendData(byte[] data) {
            try {
                client.Send(BitConverter.GetBytes(data.Length), 0, 4, SocketFlags.None);
                client.Send(data, 0, data.Length, SocketFlags.None);
            } catch (Exception) {
                //error
                this.Close();
            }
        }

        #endregion

        #region message process

        public event Action<string> NewMessage;
        public event Action<Client> RequestRemove;
        event Action<byte[]> NewPlainMessage;

        private void OnNewMessage(string msg) {
            NewMessage?.Invoke(msg);
        }

        void ReceiveData(byte[] data) {
            try {
                var type = (MessageType)data[0];
                switch (type) {
                    case MessageType.LoginPhase1:
                        string name = Information.UniversalEncoding.GetString(data);
                        var res = General.serverDatabase.GetSalt(name);
                        byte[] send_data = new byte[256];
                        Array.Copy(res.salt1, 0, send_data, 0, 128);
                        Array.Copy(res.salt2, 0, send_data, 128, 128);
                        this.Send(send_data);
                        break;
                    case MessageType.LoginPhase3:
                        //todo bcrypt decode
                        this.Send(new byte[1] { 61 });
                        break;
                    case MessageType.Broadcast:

                        break;
                    case MessageType.Text:
                        break;
                    case MessageType.Command:
                        break;
                    case MessageType.Response:
                        break;
                    case MessageType.FileHead:
                        break;
                    case MessageType.FileBody:
                        break;
                    case MessageType.E2E:
                        break;
                    case MessageType.LoginPhase2:
                    case MessageType.LoginPhase4:
                        //don't support
                        break;
                    default:
                        break;
                }
            } catch (Exception) {
                //skip
            }
        }

        object lockSendData = new object();
        void Send(byte[] data) {
            lock (lockSendData) {
                this.SecureShellSendData(data);
            }
        }

        #endregion


    }
}
