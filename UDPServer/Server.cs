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
			this._port = port < 0 ? MyDefaults.PORT_SERVER : port;
			if(dataStreamSize<0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;

			_dataStream = new byte[dataStreamSize];

			try {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				_socket.Bind(new IPEndPoint(IPAddress.Any, this._port));
				_endpointClients = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

				if (autoListens) Listen();

			} catch (Exception ex) {
				trace("Init Error: " + ex.Message);
			}
		}

		public void Close() {
			_socket.Close();
			clientList.Clear();
		}

		/////////////////////////////////////////////////////////////////////////////// Methods:

		public void Listen(AsyncCallback callback=null) {
			if(callback==null) callback = new AsyncCallback(OnMainReceivingLoop);

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

		//////////////////////////////////////////// *********************** /
		private void OnMainReceivingLoop(IAsyncResult asyncResult) {
			try {
				MyUDPInternalClient client = GetClient(asyncResult);
				trace("Received: " + client.ToString() + " - " + client.packet.clientTime + " #commands: " + client.packet.numOfCommands);

				MyPacket pk = client.packet;
				////////////////////////////////////////////

				//SendData(client.packet.EncodeTo(), client);
				SendAll(client.packet.EncodeTo());

				WaitForNextData(client.endpointIncoming); // Listen for more connections again...
			} catch (Exception ex) {
				trace("ReceiveData Error: " + ex.Message);
			}
		}
		//////////////////////////////////////////// *********************** /

		public void SendData(byte[] packetData, MyUDPInternalClient client, AsyncCallback callback = null) {
			if (callback == null) callback = new AsyncCallback(this.OnSendDataComplete);

			_socket.BeginSendTo(
				packetData, 0, packetData.Length,
				SocketFlags.None,
				client.endpointIncoming,
				callback,
				client.endpointIncoming
			);

			trace("Sending to client: " + client.ToString());
		}

		public void SendAll(byte[] packetData, AsyncCallback callback = null, EndPoint exceptEndpoint = null) {
			trace(this._clientList.Count);

			foreach (MyUDPInternalClient client in this._clientList.Values) {
				bool isSelf = exceptEndpoint == null ? false : client.endpointIncoming == exceptEndpoint;
				if (isSelf) continue;
				trace("client A: " + client.ToString());

				SendData(packetData, client, callback);
			}
		}

		private void OnSendDataComplete(IAsyncResult asyncResult) {
			try {
				socket.EndSend(asyncResult);
			} catch (Exception ex) {
				trace("SendData Error: " + ex.Message);
			}
		}

		private void WaitForNextData(EndPoint epClient, AsyncCallback callback = null) {
			if (callback == null) callback = new AsyncCallback(this.OnMainReceivingLoop); // <------ Recursively / Async receives more data?

			socket.BeginReceiveFrom(
				dataStream, 0,
				dataStream.Length,
				SocketFlags.None,
				ref epClient,
				callback,
				epClient
			);
		}

		private MyUDPInternalClient GetClient(IAsyncResult asyncResult) {
			EndPoint epClient = (EndPoint)new IPEndPoint(IPAddress.Any, MyDefaults.PORT_CLIENT); // Initialise the IPEndPoint for the clients
			socket.EndReceiveFrom(asyncResult, ref epClient); // Receive all data

			MyUDPInternalClient client;

			if (!_clientList.ContainsKey(epClient)) {
				trace("~~~ New Client! " + epClient);
				client = new MyUDPInternalClient(this);
				client.endpointIncoming = epClient;
				_clientList[epClient] = client;
			} else {
				client = _clientList[epClient];
			}

			client.ReadResult(asyncResult);
			trace("Client Endpoint: " + client.endpointIncoming);
			
			return client;
		}
	}

	///////////////////////////////////////////////////////////////

	// Structure to store the client information
	public class MyUDPInternalClient {
		public MyUDPServer server;
		public EndPoint endpointIncoming;
		public MyPacket packet;
		public string name;

		public MyUDPInternalClient(MyUDPServer server) {
			this.server = server;
			this.packet = new MyPacket();
		}

		public void ReadResult(IAsyncResult asyncResult) {
			try { 
				packet.DecodeFrom(server.dataStream); // Initialise a packet object to store the received data
			} catch(Exception e) {
				Log.trace("ReadResult Error - unable to DecodePacket data stream: \n" + e.StackTrace);
			}
		}

		public override string ToString() {
			return endpointIncoming==null ? "*no-endpoint*" : endpointIncoming.ToString();
		}
	}

}