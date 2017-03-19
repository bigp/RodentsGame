using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Net;

namespace MyUDP {
	using ClientList = Dictionary<EndPoint, MyUDPInternalClient>;

	public class MyUDPServer {
		/////////////////////////////////////////////////////////////////////////////// Privates & Getters:

		private int _port;
		public int port { get { return this._port; } }

		private byte[] _dataStream;
		public byte[] dataStream { get { return this._dataStream; } }

		private EndPoint _endpointClients;
		public EndPoint endpointClients { get { return this._endpointClients; } }

		private ClientList _clientList;
		public ClientList clientList { get { return this._clientList; } }

		private Socket _socket;
		public Socket socket { get { return this._socket; } }

		/////////////////////////////////////////////////////////////////////////////// Internal helper methods:

		public static void trace(object obj, params object[] args) {
			ConsoleColor before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("[MyUDP Server] ");
			Console.ForegroundColor = before;

			Log.trace(obj.ToString(), args);
		}

		/////////////////////////////////////////////////////////////////////////////// Constructor:

		public MyUDPServer(int port=-1, int dataStreamSize=-1, bool autoListens=true) {
			this._clientList = new ClientList();  // Initialise list of connected clients
			this._port = port < 0 ? MyDefaults.PORT : port;
			if(dataStreamSize<0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;

			_dataStream = new byte[dataStreamSize];

			try {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				_socket.Bind(new IPEndPoint(IPAddress.Any, this._port));
				_endpointClients = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

				if (autoListens) {
					Listen(new AsyncCallback(ReceiveData));
				}
			} catch (Exception ex) {
				trace("Init Error: " + ex.Message);
			}
		}

		/////////////////////////////////////////////////////////////////////////////// Methods:

		public void Listen(AsyncCallback callback) {
			// Start listening for incoming data
			_socket.BeginReceiveFrom(
				_dataStream, 0,
				_dataStream.Length,
				SocketFlags.None,
				ref _endpointClients,
				callback,
				_endpointClients
			);

			trace("Listening on port: " + port);
		}

		public void SendAll(byte[] packetData, AsyncCallback callback=null, EndPoint exceptEndpoint=null) {
			foreach (MyUDPInternalClient client in this._clientList.Values) {
				bool isSelf = exceptEndpoint==null ? false : client.endPoint == exceptEndpoint;
				if(isSelf) continue;

				// Broadcast to all logged on users
				SendData(packetData, client);
			}
		}

		public void SendData(byte[] packetData, MyUDPInternalClient client, AsyncCallback callback=null) {
			if(callback==null) callback = new AsyncCallback(this.OnSendDataComplete);

			_socket.BeginSendTo(
				packetData, 0, packetData.Length,
				SocketFlags.None,
				client.endPoint,
				callback,
				client.endPoint
			);

			trace("Sending to client: " + client.ToString());
		}

		private void OnSendDataComplete(IAsyncResult asyncResult) {
			try {
				socket.EndSend(asyncResult);
			} catch (Exception ex) {
				trace("SendData Error: " + ex.Message);
			}
		}

		internal void WaitForNextData(EndPoint epClient, AsyncCallback callback=null) {
			if(callback==null) callback = new AsyncCallback(this.ReceiveData); // <------ Recursively / Async receives more data?

			socket.BeginReceiveFrom(
				dataStream, 0,
				dataStream.Length,
				SocketFlags.None,
				ref epClient,
				callback,
				epClient
			);
		}

		private void ReceiveData(IAsyncResult asyncResult) {
			try {
				MyUDPInternalClient client = GetClient(asyncResult);
				trace("Received: " + client.ToString() + " - " + client.packet.clientTime + " #commands: " + client.packet.numOfCommands);

				MyPacket pk = client.packet;
				//////////////////////////////

				//Test: send message back to own client (self), Get packet as byte array
				SendData(client.packet.EncodePacket(), client);

				WaitForNextData(client.endPoint); // Listen for more connections again...
			} catch (Exception ex) {
				trace("ReceiveData Error: " + ex.Message);
			}
		}

		private MyUDPInternalClient GetClient(IAsyncResult asyncResult) {
			EndPoint epClient = (EndPoint)new IPEndPoint(IPAddress.Any, 0); // Initialise the IPEndPoint for the clients
			MyUDPInternalClient client;

			if (!_clientList.ContainsKey(epClient)) {
				client = new MyUDPInternalClient(this);
				client.endPoint = epClient;
			} else {
				client = _clientList[epClient];
			}

			client.ReadResult(asyncResult);

			return client;
		}

	}

	///////////////////////////////////////////////////////////////

	// Structure to store the client information
	public class MyUDPInternalClient {
		public MyUDPServer server;
		public EndPoint endPoint;
		public MyPacket packet;
		public string name;

		public MyUDPInternalClient(MyUDPServer server) {
			this.server = server;
			this.packet = new MyPacket();
		}

		public void ReadResult(IAsyncResult asyncResult) {
			try { 
				packet.DecodePacket(server.dataStream); // Initialise a packet object to store the received data
			} catch(Exception e) {
				Log.trace("ReadResult Error - unable to DecodePacket data stream: \n" + e.StackTrace);
			}
			server.socket.EndReceiveFrom(asyncResult, ref endPoint); // Receive all data
		}

		public override string ToString() {
			return endPoint==null ? "*no-endpoint*" : endPoint.ToString();
		}
	}

}