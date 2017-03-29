using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace MyUDP.Rev2Beta {

    ///////////////////////////////////////////////////////////////////////////////

    public class Client2 : Core2 {

        private MessageQueue2 _messageQueueIn;
        public MessageQueue2 messageQueueIn { get { return this._messageQueueIn; } }

        private MessageQueue2 _messageQueueOut;
        public MessageQueue2 messageQueueOut { get { return this._messageQueueOut; } }

        internal PacketStream2 _packetStream;
        public PacketStream2 packetStream { get { return this._packetStream; } }

        protected Server2 _server2;
        public bool isServerSide { get { return _server2 != null; } }

        private IPEndPoint _endpointOut;
        public IPEndPoint endpointOut { get { return this._endpointOut; } }

        private UdpClient _socket;
        public UdpClient socket { get { return this._socket; } }

        private string _host;
        public string host { get { return this._host; } }

        ///////////////////////////////////////////////////////////////////////////////

        public Client2(int port = -1, int dataStreamSize = -1) : base(port) {
            if (dataStreamSize < 0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;

            _packetStream = new PacketStream2(dataStreamSize);
            _messageQueueIn = new MessageQueue2();
        }

        public virtual void Close() {
            if (_socket == null) return;
            _socket.Close();
        }

        ///////////////////////////////////////////////////////////////////////////////

        public void SetAsServerSide(Server2 server2) {
            _server2 = server2;
        }

        public void SetAsClientSide(string hostname = "127.0.0.1", bool autoConnect=true) {
            _host = hostname;

            IPAddress ipAddr;

            if (!IPAddress.TryParse(_host, out ipAddr)) {
                IPAddress[] addresses = Dns.GetHostAddresses(_host);
                if (addresses.Length == 0) {
                    traceError("Cannot resolve the host: " + _host);
                    return;
                }

                ipAddr = addresses[0];
            }

            _endpointOut = new IPEndPoint(ipAddr, _port);

            if (autoConnect) Connect(_port);
        }

        ///////////////////////////////////////////////////////////////////////////////

        public virtual void Connect(int port = -1) {
            if (port < 0) port = this._port;

            try {
                Close();
                _socket = new UdpClient(port);
            } catch (Exception ex) {
                if (ex.Message.Contains("Only one usage")) {
                    //If it's a blocked port, try the next one:
                    Connect(port + 1);
                } else {
                    traceError("Connection ERROR! ------ " + ex.Message);
                }

                return;
            }

            if (_socket != null) __Listen();
        }

        private void __Listen(AsyncCallback callback = null) {
            _socket.BeginReceive(__Received, _endpointIn);
        }

        private void __Received(IAsyncResult asyncResult) {
            try {
                byte[] bytesReceived = _socket.EndReceive(asyncResult, ref _endpointIn);

                _messageQueueIn.AddBytes(bytesReceived);

                //_packet.Decode(bytesReceived);
                //if (OnPacketReceived != null) OnPacketReceived(_packet);

            } catch (Exception ex) {
                traceError("OnDataReceived error: " + ex.Message);
                //if (ex.Message.Contains("forcibly")) {
                //    //_isConnected = false;
                //}
            }

            __Listen();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////

    
}
