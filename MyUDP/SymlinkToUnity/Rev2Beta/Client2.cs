using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace MyUDP.Rev2Beta {

    ///////////////////////////////////////////////////////////////////////////////

    public class Client2 : Core2 {
        private static object thisLock = new object();

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

        public Action<PacketStream2> OnPacketPreSend;
        public Action<byte[]> OnClientReceivedBytes;
        //public Action<PacketStream2> OnPacketPreSend;

        ///////////////////////////////////////////////////////////////////////////////

        public Client2(string hostname = "127.0.0.1", int port = -1, int dataStreamSize = -1) : base(port) {
            if (dataStreamSize < 0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;

            _host = hostname;
            _packetStream = new PacketStream2(dataStreamSize);
        }

        public virtual void Close() {
            if (_socket == null) return;
            _socket.Close();
        }

        ///////////////////////////////////////////////////////////////////////////////

        public void SetAsServerSide(Server2 server2, int port = -1) {
            _port = port < 0 ? MyDefaults.SERVER_PORT : port;
            _server2 = server2;
        }

        public void SetAsClientSide(bool autoConnect=true) {
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
                _socket = new UdpClient(); //port
                _socket.Connect(_endpointOut);

            } catch (Exception ex) {
                traceError("Connect failed: " + ex.Message);
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
            try {
                _socket.BeginReceive(__Received, _endpointIn);
            } catch(Exception ex) {
                traceError("__Listen ERROR: " + ex.Message);
            }
        }

        private void __Received(IAsyncResult asyncResult) {
            lock (thisLock) {
                try {
                    byte[] bytesFromServer = _socket.EndReceive(asyncResult, ref _endpointIn);

                    trace(_endpointIn + " <<<< RECEIVED");

                    if (bytesFromServer.Length > 0) {
                        if (OnClientReceivedBytes != null) {
                            OnClientReceivedBytes(bytesFromServer);
                        }
                    }
                } catch (Exception ex) {
                    traceError("OnDataReceived error: " + _endpointIn + " : " + ex.Message);
                }

                __Listen();
            }
        }

        public void Send(PacketStream2 stream=null) {
            if (_socket == null) {
                trace("Cannot send yet, socket not ready!");
                return;
            }

            if(stream==null) stream = this.packetStream;

            lock (thisLock) {
                try {
                    if (OnPacketPreSend != null) OnPacketPreSend(stream);

                    trace(_endpointOut + "        >>>>>>>> SENDING");
                    _socket.BeginSend(stream.byteStream, stream.byteLength, __OnSendComplete, _endpointOut);
                } catch (Exception ex) {
                    traceError("Send error: " + ex.ToString());
                }
            }
        }

        private void __OnSendComplete(IAsyncResult ar) {
            _socket.EndSend(ar);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////

    
}
