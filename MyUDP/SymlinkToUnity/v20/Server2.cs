using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace MyUDP.v20 {

    using ClientList = Dictionary<EndPoint, Client2>;

    public class Server2 : Core2 {
        public bool isClearConsoleOnReceive = false;
        private Object thisLock = new Object();

        private bool _listening = false;

        private ClientList _clientList;
        public ClientList clientList { get { return _clientList; } }

        private EndPoint _reusedEndpoint;

        protected int _receivedBytesLength = 0;
        protected byte[] _bytesFromClient;
        protected Socket _socket;
        public Socket socket { get { return this._socket; } }

        public Action<Client2> OnNewClient;
        public Action<Client2> OnReceivedFromClient;
        public Func<EndPoint, bool> OnValidateEndpoint;
        public Func<Client2, bool> OnValidateClient;

        public Server2(int port = -1,
                        int dataStreamSize = -1,
                        bool autoListens = true)
                        : base(port) {

            if(dataStreamSize<=0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;
            
            _reusedEndpoint = (EndPoint)new IPEndPoint(IPAddress.Any, MyDefaults.CLIENT_PORT);
            _clientList = new ClientList();
            _bytesFromClient = new byte[dataStreamSize];

            try {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.Bind( new IPEndPoint(IPAddress.Any, _port) );
                trace("Server socket port: " + this._port);

                if (autoListens) BeginListen();
            } catch(Exception ex) {
                traceError("Could not initialize the Server: " + ex.Message);
            }
        }

        public void Close() {
            _socket.Close();
            _clientList.Clear();
        }

        public void BeginListen() {
            if(_listening) return;

            _listening = true;
            __Listen();
        }
        
        private void __Listen() {
            EndPoint ep = (EndPoint)_endpointIn;

            // Start listening for incoming data
            _socket.BeginReceiveFrom(
                _bytesFromClient, 0, _bytesFromClient.Length,
                SocketFlags.None,
                ref ep,
                __Received,
                _endpointIn
            );
        }

        private void __Received(IAsyncResult asyncResult) {
            lock (thisLock) {
#if JUST_CONSOLE
                if (isClearConsoleOnReceive) Console.Clear();
#endif
                try {
                    Client2 client = GetClient(asyncResult);

                    if(client!=null) {
                        if (client.OnReceivedBytes != null) client.OnReceivedBytes(_bytesFromClient);
                        if (OnReceivedFromClient != null) OnReceivedFromClient(client);
                    }

                    __Listen();
                } catch (Exception ex) {
                    trace("ReceiveData Error: " + ex.Message);
                }
            }
        }

        public void SendData(PacketStream2 stream, Client2 client) {
            __SendData(stream, client);
        }

        public void SendAll(PacketStream2 stream, EndPoint endpointException = null) {
            foreach (Client2 client in this._clientList.Values) {
                bool isSelf = endpointException == null ? false : client._endpointIn == endpointException;
                if (isSelf) continue;

                __SendData(stream, client);
            }
        }

        private void __SendData(PacketStream2 stream, Client2 client) {
            _socket.BeginSendTo(
                stream.byteStream, 0, stream.byteLength,
                SocketFlags.None,
                client._endpointIn,
                __OnSendDataComplete,
                client._endpointIn
            );
        }

        private void __OnSendDataComplete(IAsyncResult asyncResult) {
            lock (thisLock) {
                try {
                    socket.EndSend(asyncResult);
                } catch (Exception ex) {
                    trace("SendData Error: " + ex.Message);
                }
            }
        }

        private Client2 GetClient(IAsyncResult asyncResult) {
            // Receive all data & assigns the IPEndPoint of the incoming client:
            _receivedBytesLength = socket.EndReceiveFrom(asyncResult, ref _reusedEndpoint);
            if(_receivedBytesLength == 0) return null;
            if(OnValidateEndpoint!=null && !OnValidateEndpoint(_reusedEndpoint)) return null;

            Client2 client;

            if (!_clientList.ContainsKey(_reusedEndpoint)) {
                client = new Client2();
                client._endpointIn = (IPEndPoint)_reusedEndpoint;
                client.SetAsServerSide(this);

                _clientList[_reusedEndpoint] = client;

                if (OnNewClient != null) OnNewClient(client);

            } else {
                client = _clientList[_reusedEndpoint];
            }

            if (OnValidateClient!=null && !OnValidateClient(client)) return null;

            return client;
        }

        internal void ForgetClient(IPEndPoint endpointIn) {
            clientList.Remove(endpointIn);
        }
    }
}
