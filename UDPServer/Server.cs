using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Net;

namespace MyUDP {
	using ClientList = Dictionary<EndPoint, MyUDPClient>;

	public class MyUDPServer {
		/////////////////////////////////////////////////////////////////////////////// Constants & Defaults:
		public static int DATA_STREAM_SIZE = 1024;
		public static int PORT = 11000;

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
			Console.Write("[MyUDP] ");
			Console.ForegroundColor = before;

			Log.trace(obj.ToString(), args);
		}

		internal static MyUDPPacket CreateTestPacket(EnumPacketPart id, string name, string msg) {
			MyUDPPacket packet = new MyUDPPacket();
			packet.Identifier = id;
			packet.Name = "[" + name + "]";
			packet.Message = msg;
			return packet;
		}

		/////////////////////////////////////////////////////////////////////////////// Constructor:

		public MyUDPServer(int port=-1, int dataStreamSize=-1, bool autoListens=true) {
			this._clientList = new ClientList();  // Initialise list of connected clients
			this._port = port < 0 ? PORT : port;
			if(dataStreamSize<0) dataStreamSize = DATA_STREAM_SIZE;

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
			foreach (MyUDPClient client in this._clientList.Values) {
				bool isSelf = exceptEndpoint==null ? false : client.endPoint == exceptEndpoint;
				if(isSelf) continue;

				// Broadcast to all logged on users
				SendData(packetData, client);
			}
		}

		public void SendData(byte[] packetData, MyUDPClient client, AsyncCallback callback=null) {
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
			if(callback==null) callback = new AsyncCallback(this.ReceiveData);

			socket.BeginReceiveFrom(
				dataStream, 0,
				dataStream.Length,
				SocketFlags.None,
				ref epClient,
				callback, // <------ Recursively / Async receives more data?
				epClient
			);
		}

		private void ReceiveData(IAsyncResult asyncResult) {
			try {
				MyUDPClient client = GetClient(asyncResult);
				trace("Received: " + client.ToString() + " - " + client.packet.Message);

				string str = "";
				MyUDPPacket pk = client.packet;

				switch (client.packet.Identifier) {
					case EnumPacketPart.Message: str = string.Format("Sender {0}: {1}", pk.Name, pk.Message); break;
					default: str = "*null or unknown*"; break;
				}

				//Test: send message back to own client (self), Get packet as byte array
				MyUDPPacket packetOut = CreateTestPacket(client.packet.Identifier, client.packet.Name, str);
				SendData(packetOut.GetDataStream(), client);

				WaitForNextData(client.endPoint); // Listen for more connections again...
			} catch (Exception ex) {
				trace("ReceiveData Error: " + ex.Message);
			}
		}

		private MyUDPClient GetClient(IAsyncResult asyncResult) {
			EndPoint epClient = (EndPoint)new IPEndPoint(IPAddress.Any, 0); // Initialise the IPEndPoint for the clients
			MyUDPClient client;

			if (!_clientList.ContainsKey(epClient)) {
				client = new MyUDPClient(this);
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
	public class MyUDPClient {
		public MyUDPServer server;
		public EndPoint endPoint;
		public MyUDPPacket packet;
		public string name;

		public MyUDPClient(MyUDPServer server) {
			this.server = server;
			this.packet = new MyUDPPacket();
		}

		public void ReadResult(IAsyncResult asyncResult) {
			packet.DecodePacket(server.dataStream); // Initialise a packet object to store the received data
			server.socket.EndReceiveFrom(asyncResult, ref endPoint); // Receive all data
		}

		public override string ToString() {
			return endPoint==null ? "*no-endpoint*" : endPoint.ToString();
		}
	}

	//bool isLoginStatus = packetOut.ChatDataIdentifier == EnumPacketPart.LogIn;

	//foreach (MyClient client in this.clientList) {
	//	bool isSelf = client.endPoint == endpointClients;
	//	//if(isLoginStatus) continue; // isSelf

	//	Program.trace("Sending to client: " + client);

	//	// Broadcast to all logged on users
	//	serverSocket.BeginSendTo(
	//		data, 0,
	//		data.Length,
	//		SocketFlags.None,
	//		client.endPoint,
	//		new AsyncCallback(this.SendData),
	//		client.endPoint
	//	);
	//}

}