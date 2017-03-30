﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace MyUDP.Rev2Beta {

    using ClientList = Dictionary<EndPoint, Client2>;

    public class Server2 : Core2 {
        public bool isClearConsoleOnReceive = false;
        private Object thisLock = new Object();

        private bool _listening = false;

        private ClientList _clientList;
        public ClientList clientList { get { return _clientList; } }

        private EndPoint _reusedEndpoint;

        private byte[] _receivedBytes;
        private Socket _socket;
        public Socket socket { get { return this._socket; } }

        public Action<Client2> OnNewClient;
        public Func<EndPoint, bool> OnValidateEndpoint;
        public Func<Client2, bool> OnValidateClient;
        //public Action<PacketStream2> OnPacketDecoded;
        //public Action<PacketStream2> OnPacketEncoded;
        public Action<Client2> OnDataReceived;

        public Server2(int port = -1,
                        int dataStreamSize = -1,
                        bool autoListens = true)
                        : base() {
            if(dataStreamSize<=0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;
            if(port<0) port = MyDefaults.SERVER_PORT;

            _port = port;
            _reusedEndpoint = (EndPoint)new IPEndPoint(IPAddress.Any, MyDefaults.CLIENT_PORT);
            _clientList = new ClientList();
            _receivedBytes = new byte[dataStreamSize];

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

            trace("Listening... " + _endpointIn);
            // Start listening for incoming data
            byte[] bytes = _receivedBytes;

            _socket.BeginReceiveFrom(
                bytes, 0, bytes.Length,
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

                    trace("client: " + client);
                    if(client!=null) {
                        client.messageQueueIn.AddBytes(_receivedBytes);

                        trace(client.endpointIn + " : " + client.endpointOut);
                        if (OnDataReceived != null) OnDataReceived(client);

                        //try {
                        //    packet.Decode(_byteStream); // Initialise a packet object to store the received data
                        //    if (OnPacketDecoded != null) OnPacketDecoded(packet);
                        //} catch (Exception e) {
                        //    Log.trace("ReadResult Error - unable to DecodePacket data stream: \n" + e.StackTrace);
                        //}
                    }

                    __Listen();

                } catch (Exception ex) {
                    trace("ReceiveData Error: " + ex.Message);
                }
            }
        }

        private Client2 GetClient(IAsyncResult asyncResult) {
            // Receive all data & assigns the IPEndPoint of the incoming client:
            int byteCount = socket.EndReceiveFrom(asyncResult, ref _reusedEndpoint);
            if(byteCount==0) return null;
            if(OnValidateEndpoint!=null && !OnValidateEndpoint(_reusedEndpoint)) return null;

            Client2 client;

            if (!_clientList.ContainsKey(_reusedEndpoint)) {
                client = new Client2(); //this
                client._endpointIn = (IPEndPoint)_reusedEndpoint;
                client.SetAsServerSide(this);

                _clientList[_reusedEndpoint] = client;

                if (OnNewClient != null) OnNewClient(client);

            } else {
                client = _clientList[_reusedEndpoint];
            }

            if(OnValidateClient!=null && !OnValidateClient(client)) return null;

            return client;
        }

        internal void ForgetClient(IPEndPoint endpointIn) {
            clientList.Remove(endpointIn);
        }
    }
}
