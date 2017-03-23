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
		/////////////////////////////////////////////////////////////////////////////// Privates & Getters:

		private ClientList _clientList;
		public ClientList clientList { get { return this._clientList; } }

		private Socket _socket;
		public Socket socket { get { return this._socket; } }

		public Action<MyUDPPacket> OnPacketDecoded;
		public Action<MyUDPPacket> OnPacketEncoded;
		public Action<MyUDPPacket, MyUDPServerClient> OnDataReceived;

		public bool isClearConsoleOnReceive = false;

		/////////////////////////////////////////////////////////////////////////////// Internal helper methods:

#if !UNITY_EDITOR
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
			Log.trace("[MyUDP Server] " + obj.ToString(), args);
		}

		public static void traceError(object obj, params object[] args) {
			Log.trace("[MyUDP Server] ERROR: " + obj.ToString(), args);
		}
#endif
		/////////////////////////////////////////////////////////////////////////////// Constructor:

		public MyUDPServer(int port=-1, int dataStreamSize=-1, bool autoListens=true) : base(port, dataStreamSize) {
			this._clientList = new ClientList();  // Initialise list of connected clients
			
			try {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				_socket.Bind(new IPEndPoint(IPAddress.Any, _port));
				
				if (autoListens) {
					trace("Listening on port: " + _port);
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
			if(isClearConsoleOnReceive) Console.Clear();

			try {
				MyUDPServerClient client = GetClient(asyncResult);
				//trace("Received: " + client.ToString() + " - " + client.packetIn.clientTimeFormatted + " #commands: " + client.packet.numOfCommands);

				if(OnDataReceived!=null) OnDataReceived(packet, client);

				bool bytesOK = false;
				try {
					_packet.Encode(_byteStream);
					if(OnPacketEncoded!=null) OnPacketEncoded(_packet);
					bytesOK = true;
				} catch (Exception ex) {
					Log.traceError("Packet Encode error: " + ex.Message);
				}

				if(bytesOK) SendAll(_byteStream, _packet.byteLength);

				//WaitForNextData(client.endpointIncoming); // Listen for more connections again...
				Listen();
			} catch (Exception ex) {
				trace("ReceiveData Error: " + ex.Message);
			}
		}

		//**************************************************************************/////

		private MyUDPServerClient GetClient(IAsyncResult asyncResult) {
			// Initialise the IPEndPoint for the clients
			EndPoint epClient = (EndPoint)new IPEndPoint(IPAddress.Any, MyDefaults.CLIENT_PORT);

			// Receive all data & assigns the IPEndPoint of the incoming client:
			int byteCount = socket.EndReceiveFrom(asyncResult, ref epClient);

			MyUDPServerClient client;
			string pre = epClient + " (bytes: {0})";

			if (!_clientList.ContainsKey(epClient)) {
				trace(pre + " ** New Client! **", byteCount);
				client = new MyUDPServerClient(this);
				client._endpointIn = (IPEndPoint)epClient;
				client.SetPacketType(this.packet.GetType());
				_clientList[epClient] = client;

				if (client.packet == null) {
					throw new Exception("client.packet is null! Probably didn't initialize correctly.");
				}
			} else {
				trace(pre, byteCount);
				client = _clientList[epClient];
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
			trace("----> Client: " + client.ToString() + " bytes: " + length);

			__SendData(bytesOut, length, client, callback);
		}

		public void SendAll(byte[] bytesOut, int length, AsyncCallback callback = null, EndPoint endpointException = null) {
			trace(this._clientList.Count);

			trace("SendAll...");
			foreach (MyUDPServerClient client in this._clientList.Values) {
				bool isSelf = endpointException == null ? false : client._endpointIn == endpointException;
				if (isSelf) continue;

				trace("   --> Client: " + client.ToString() + " bytes: " + length);
				__SendData(bytesOut, length, client, callback);
			}
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
			try {
				socket.EndSend(asyncResult);
			} catch (Exception ex) {
				trace("SendData Error: " + ex.Message);
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