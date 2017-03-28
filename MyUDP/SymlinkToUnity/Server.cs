using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Net;

namespace MyUDP {
	using Core;
	using ClientList = Dictionary<EndPoint, MyUDPServerClient>;

	public class MyUDPServer : MyUDPCore {
        private Object thisLock = new Object();
        
        /////////////////////////////////////////////////////////////////////////////// Privates & Getters:

        private ClientList _clientList;
		public ClientList clientList { get { return this._clientList; } }

		private Socket _socket;
		public Socket socket { get { return this._socket; } }

        //Not sure this reused endpoint object is a 'good' thing, might be comparing by ref incorrectly :/
        private EndPoint _reusedEndpoint;

        public Action<MyUDPServerClient> OnNewClient;
        public Action<MyUDPPacket> OnPacketDecoded;
		public Action<MyUDPPacket> OnPacketEncoded;
		public Action<MyUDPPacket, MyUDPServerClient> OnDataReceived;

		public bool isClearConsoleOnReceive = false;

		/////////////////////////////////////////////////////////////////////////////// Internal helper methods:

#if JUST_CONSOLE
		public static void trace(object obj, params object[] args) {
			ConsoleColor before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("[MyUDP Server] ");
			Console.ForegroundColor = before;
			Log.trace(obj.ToString(), args);
		}

		public static void traceError(object obj, params object[] args) {
			ConsoleColor before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("[MyUDP Server] ");
			Console.ForegroundColor = before;
			Log.traceError(obj.ToString(), args);
		}
#else
		public static void trace(object obj, params object[] args) {
			//Log.trace("[MyUDP Server] " + obj.ToString(), args);
		}

		public static void traceError(object obj, params object[] args) {
			//Log.trace("[MyUDP Server] ERROR: " + obj.ToString(), args);
		}
#endif
        /////////////////////////////////////////////////////////////////////////////// Constructor:

        public MyUDPServer(int port=-1, int dataStreamSize=-1, bool autoListens=true) : base(port, dataStreamSize) {
			_clientList = new ClientList();  // Initialise list of connected clients
            _reusedEndpoint = (EndPoint)new IPEndPoint(IPAddress.Any, MyDefaults.CLIENT_PORT);

            try {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				_socket.Bind(new IPEndPoint(IPAddress.Any, _port));
				
				if (autoListens) {
                    Listen();
				}
			} catch (Exception ex) {
				trace("Init Error: " + ex.Message);
			}
		}

        /////////////////////////////////////////////////////////////////////////////// Methods:

        public void Close() {
			_socket.Close();
			clientList.Clear();
		}

		public void Listen(AsyncCallback callback=null) {
			if(callback==null) callback = this.OnMainReceivingLoop;
			
			EndPoint ep = (EndPoint) _endpointIn;

			// Start listening for incoming data
			_socket.BeginReceiveFrom(
				_byteStream, 0,
				_byteStream.Length,
				SocketFlags.None,
				ref ep,
				callback,
				_endpointIn
			);
		}

		//**************************************************************************\\\\\

		private void OnMainReceivingLoop(IAsyncResult asyncResult) {
            lock (thisLock) {
#if JUST_CONSOLE
                if (isClearConsoleOnReceive) Console.Clear();
#endif
                try {
                    MyUDPServerClient client = GetClient(asyncResult);

                    if (OnDataReceived != null) OnDataReceived(packet, client);

                    //Demo();

                    Listen();
                } catch (Exception ex) {
                    trace("ReceiveData Error: " + ex.Message);
                }
            }
		}

        private void Demo() {
            bool bytesOK = false;
            try {
                _packet.Encode(_byteStream);
                if (OnPacketEncoded != null) OnPacketEncoded(_packet);
                bytesOK = true;
            } catch (Exception ex) {
                Log.traceError("Packet Encode error: " + ex.Message);
            }

            if (bytesOK) SendAll(_byteStream, _packet.byteLength);
        }

		//**************************************************************************/////

		private MyUDPServerClient GetClient(IAsyncResult asyncResult) {
            // Receive all data & assigns the IPEndPoint of the incoming client:
			int byteCount = socket.EndReceiveFrom(asyncResult, ref _reusedEndpoint);

			MyUDPServerClient client;

			if (!_clientList.ContainsKey(_reusedEndpoint)) {
				client = new MyUDPServerClient(this);
				client._endpointIn = (IPEndPoint)_reusedEndpoint;
				client.SetPacketType(this.packet.GetType());
				
				if (client.packet == null) {
					throw new Exception("client.packet is null! Probably didn't initialize correctly.");
				} else {
                    _clientList[_reusedEndpoint] = client;
                }

                if(OnNewClient!=null) OnNewClient(client);

            } else {
				client = _clientList[_reusedEndpoint];
			}

			try {
				packet.Decode(_byteStream); // Initialise a packet object to store the received data
				if (OnPacketDecoded != null) OnPacketDecoded(packet);
			} catch (Exception e) {
				Log.trace("ReadResult Error - unable to DecodePacket data stream: \n" + e.StackTrace);
			}

			return client;
		}

		//////////////////////////////////////////// *********************** /

		public void SendData(byte[] bytesOut, int length, MyUDPServerClient client, AsyncCallback callback = null) {
			__SendData(bytesOut, length, client, callback);
		}

		public void SendAll(byte[] bytesOut, int length, AsyncCallback callback = null, EndPoint endpointException = null) {
			foreach (MyUDPServerClient client in this._clientList.Values) {
				bool isSelf = endpointException == null ? false : client._endpointIn == endpointException;
				if (isSelf) continue;

				__SendData(bytesOut, length, client, callback);
			}
		}

        public bool ForgetClient(EndPoint epClient) {
            if(clientList.ContainsKey(epClient)) {
                clientList.Remove(epClient);
                return true;
            }

            return false;
        }

        public bool ForgetClient(MyUDPServerClient client) {
            EndPoint epFound = null;
            foreach(EndPoint ep in clientList.Keys) {
                if(ep == client.endpointIn) {
                    epFound = ep;
                    break;
                }
            }

            if(epFound!=null) {
                clientList.Remove(epFound);
                return true;
            }
            return false;
        }

        private void __SendData(byte[] bytesOut, int length, MyUDPServerClient client, AsyncCallback callback = null) {
			if (callback == null) callback = this.OnSendDataComplete;

			_socket.BeginSendTo(
				bytesOut, 0, length,
				SocketFlags.None,
				client._endpointIn,
				callback,
				client._endpointIn
			);
		}

		private void OnSendDataComplete(IAsyncResult asyncResult) {
            lock (thisLock) {
                try {
                    socket.EndSend(asyncResult);
                } catch (Exception ex) {
                    trace("SendData Error: " + ex.Message);
                }
            }
		}
	}

	///////////////////////////////////////////////////////////////

	// Structure to store the client information
	public class MyUDPServerClient : MyUDPCore {
		public MyUDPServer server;
		public string name;

		public MyUDPServerClient(MyUDPServer server) : base(-1, -1) {
			this.server = server;
		}
	}

}