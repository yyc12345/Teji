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

            SecureShellReceiveData();
        }

        Socket client;
        public string Guid { get; private set; }
        public string UserName { get; private set; }
        string tryUserName = "";
        public string EndPoint {
            get {
                return client.RemoteEndPoint.ToString();
            }
        }

        public void Close() {
            client.Close();
            RequestRemove?.Invoke(this);
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

        //public event Action<string> NewMessage;
        public event Action<Client, string, string> TextMessage;
        public event Action<Client, string> CommandMessage;
        public event Action<Client, string> RequestMessage;
        public event Action<Client, string, int, int, int> FileHeadMessage;
        public event Action<Client, string, int, byte[]> FileBodyMessage;
        public event Action<Client, string, byte[]> E2EMessage;
        public event Action<Client> RequestRemove;

        event Action<byte[]> NewPlainMessage;
        
        void ReceiveData(byte[] plain_data) {
            try {
                var type = (MessageType)plain_data[0];
                var data = new byte[plain_data.Length - 1];
                Array.Copy(plain_data, 1, data, 0, plain_data.Length - 1);
                switch (type) {
                    case MessageType.LoginPhase1:
                        if (this.UserName != "" || this.tryUserName != "") return;
                        string name = Information.UniversalEncoding.GetString(data);
                        var res = General.serverDatabase.GetSalt(name);
                        if (!res.isOK) return;
                        byte[] send_data = new byte[256];
                        Array.Copy(res.salt1, 0, send_data, 0, 128);
                        Array.Copy(res.salt2, 0, send_data, 128, 128);
                        this.Send(send_data);
                        this.tryUserName = name;
                        break;
                    case MessageType.LoginPhase3:
                        if (this.tryUserName == "" || this.UserName != "") return;
                        //todo bcrypt decode
                        //ok
                        this.Send(new byte[1] { 61 });
                        this.UserName = this.tryUserName;
                        break;
                    case MessageType.TextIn:
                        if (this.UserName == "" || this.tryUserName == "") return;
                        int room_length = BitConverter.ToInt32(data, 0);
                        this.TextMessage?.Invoke(this, Information.UniversalEncoding.GetString(data, 4, room_length),
                            Information.UniversalEncoding.GetString(data, 4 + room_length, data.Length - 4 - room_length));
                        break;
                    case MessageType.Command:
                        if (this.UserName == "" || this.tryUserName == "") return;
                        this.CommandMessage?.Invoke(this, Information.UniversalEncoding.GetString(data));
                        break;
                    case MessageType.Request:
                        if (this.UserName == "" || this.tryUserName == "") return;
                        this.RequestMessage?.Invoke(this, data.ConvertToString());
                        break;
                    case MessageType.FileHead:
                        if (this.UserName == "" || this.tryUserName == "") return;
                        var cache = new byte[256];
                        Array.Copy(data, 0, cache, 0, 256);
                        this.FileHeadMessage?.Invoke(this, cache.ConvertToString(),
                            BitConverter.ToInt32(data, 256), BitConverter.ToInt32(data, 256 + 4), BitConverter.ToInt32(data, 256 + 4 + 4));
                        break;
                    case MessageType.FileBody:
                        if (this.UserName == "" || this.tryUserName == "") return;
                        var cache2 = new byte[256];
                        Array.Copy(data, 0, cache2, 0, 256);
                        var file_data = new byte[data.Length - 256 - 4];
                        Array.Copy(data, 256 + 4, file_data, 0, data.Length - 256 - 4);
                        this.FileBodyMessage?.Invoke(this, cache2.ConvertToString(), BitConverter.ToInt32(data, 256), file_data);
                        break;
                    case MessageType.E2EIn:
                        if (this.UserName == "" || this.tryUserName == "") return;
                        var to_length = BitConverter.ToInt32(data, 0);
                        var to_user = Information.UniversalEncoding.GetString(data, 4, to_length);
                        var e2e_data = new byte[data.Length - 4 - to_length];
                        Array.Copy(data, 4 + to_length, e2e_data, 0, data.Length - 4 - to_length);
                        this.E2EMessage?.Invoke(this, to_user, e2e_data);
                        break;
                    case MessageType.LoginPhase2:
                    case MessageType.LoginPhase4:
                    case MessageType.TextOut:
                    case MessageType.Response:
                    case MessageType.Broadcast:
                    case MessageType.E2EOut:
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

        public void SendTextOut(string room,string user,long time_stamp,string words) {

        }

        public void SendResponse(string words) {

        }

        public void SendBroadcast(string words) {

        }

        public void SendRequest(string guid) {

        }

        public void SendFileHead(string guid,int section_count,int section_length,int  last_section_length) {

        }

        public void SendFileBody(string guid, int index, byte[] section) {

        }

        public void SendE2EOut(string from, byte[] data) {

        }

        #endregion


    }
}
