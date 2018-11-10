using System;
using System.Collections.Generic;
using System.Text;
using TejiLib;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TejiClient {

    public class Server {

        public Server(Socket s, string guid, string userName, string password) {
            server = s;
            this.Guid = guid;
            this.UserName = userName;
            this.password = password;

            NewPlainMessage += this.ReceiveData;

            SecureShellReceiveData();
            this.StartDialog();
        }

        Socket server;
        public string Guid { get; private set; }
        public string UserName { get; private set; }
        string password { get; set; }
        bool ConnectionOK = false;
        public string EndPoint {
            get {
                return server.RemoteEndPoint.ToString();
            }
        }

        public void Close() {
            server.Close();
            RequestRemove?.Invoke(this);
        }

        void StartDialog() {

        }

        #region secure shell


        void SecureShellReceiveData() {
            try {
                byte[] length_buffer = new byte[4];
                int length = 0;
                byte[] data_buffer;
                while (true) {
                    server.Receive(length_buffer, 0, 4, SocketFlags.None);
                    length = BitConverter.ToInt32(length_buffer, 0);
                    data_buffer = new byte[length];
                    server.Receive(data_buffer, 0, 4, SocketFlags.None);
                    this.NewPlainMessage?.Invoke(data_buffer);
                }
            } catch (Exception) {
                //error
                this.Close();
            }
        }

        void SecureShellSendData(byte[] data) {
            try {
                server.Send(BitConverter.GetBytes(data.Length), 0, 4, SocketFlags.None);
                server.Send(data, 0, data.Length, SocketFlags.None);
            } catch (Exception) {
                //error
                this.Close();
            }
        }

        #endregion

        #region message process

        //public event Action<string> NewMessage;
        public event Action<Server, string, string, long, string> TextMessage;
        public event Action<Server, string> ResponseMessage;
        public event Action<Server, string> RequestMessage;
        public event Action<Server, string> BroadcastMessage;
        public event Action<Server, string, int, int, int> FileHeadMessage;
        public event Action<Server, string, int, byte[]> FileBodyMessage;
        public event Action<Server, string, byte[]> E2EMessage;
        public event Action<Server> RequestRemove;

        event Action<byte[]> NewPlainMessage;

        void ReceiveData(byte[] plain_data) {
            try {
                var type = (MessageType)plain_data[0];
                var data = new byte[plain_data.Length - 1];
                Array.Copy(plain_data, 1, data, 0, plain_data.Length - 1);
                switch (type) {
                    case MessageType.LoginPhase2:
                        break;
                    case MessageType.LoginPhase4:
                        break;
                    case MessageType.TextOut:
                        break;
                    case MessageType.Response:
                        break;
                    case MessageType.Broadcast:
                        break;
                    case MessageType.Request:
                        break;
                    case MessageType.FileHead:
                        break;
                    case MessageType.FileBody:
                        break;
                    case MessageType.E2EOut:
                        break;
                    case MessageType.LoginPhase1:
                    case MessageType.LoginPhase3:
                    case MessageType.TextIn:
                    case MessageType.Command:
                    case MessageType.E2EIn:
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
