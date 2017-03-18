using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Net;

public class MyServer {
	public static int PACKET_SIZE = 1024;
	public static int PORT = 11000;

	private List<MyClient> clientList; // Listing of clients
	private MyUDPSocket serverUDP; // Server socket
	private List<EndPoint> clientsWhoSent;

	public MyServer() {
		Program.trace("Server Init.");

		this.clientList = new List<MyClient>();  // Initialise the ArrayList of connected clients
		this.clientsWhoSent = new List<EndPoint>();

		serverUDP = new MyUDPSocket(PORT);
		serverUDP.Listen(new AsyncCallback(ReceiveData));
	}

	private void ReceiveData(IAsyncResult asyncResult) {
		try {
			MyPacket packetIn = new MyPacket(serverUDP.dataStream); // Initialise a packet object to store the received data
			EndPoint epClient = (EndPoint)new IPEndPoint(IPAddress.Any, 0); // Initialise the IPEndPoint for the clients

			serverUDP.socket.EndReceiveFrom(asyncResult, ref epClient); // Receive all data

			if (!clientsWhoSent.Contains(epClient)) {
				//clientsWhoSent.Add(epClient);
			}

			Program.trace("Client sent data: " + epClient.ToString());

			string str = "";

			switch (packetIn.Identifier) {
				case EnumPacketPart.Message:
					str = string.Format("Sender {0}: {1}", packetIn.Name, packetIn.Message);
					break;


				case EnumPacketPart.LogIn:
					MyClient client = new MyClient(epClient, packetIn.Name); // Populate client object
					this.clientList.Add(client); // Add client to list
					str = string.Format("-- {0} is online --", packetIn.Name);
					break;


				case EnumPacketPart.LogOut:
					foreach (MyClient c in this.clientList) {
						// Remove current client from list
						if (c.endPoint.Equals(epClient)) {
							this.clientList.Remove(c);
							break;
						}
					}

					str = string.Format("-- {0} has gone offline --", packetIn.Name);
					break;


				default:
					str = "*null or unknown*";
					break;
			}

			MyPacket packetOut = new MyPacket();
			packetOut.Identifier = packetIn.Identifier;
			packetOut.Name = "[" + packetIn.Name + "]";
			packetOut.Message = str;

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

			//Test: send message back to own client (self), Get packet as byte array
			serverUDP.SendData(packetOut.GetDataStream(), epClient);

			// Listen for more connections again...
			serverUDP.WaitForNextData(epClient, new AsyncCallback(this.ReceiveData));

		} catch (Exception ex) {
			Program.trace("ReceiveData Error: " + ex.Message);
		}
	}
}

public class MyUDPSocket {
	private Socket _socket;
	public Socket socket { get { return this._socket; } }

	private int _dataStreamLen;
	private byte[] _dataStream;
	public byte[] dataStream { get { return this._dataStream; } }

	private EndPoint _endpointSender;
	public EndPoint endpointSender { get { return this._endpointSender; } }

	public MyUDPSocket(int port, int dataStreamLen = 1024) {
		_dataStream = new byte[dataStreamLen];
		this._dataStreamLen = dataStreamLen;

		try {
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Initialise the socket
			_socket.Bind(new IPEndPoint(IPAddress.Any, port));

			// Initialise the EndPoint for the clients
			_endpointSender = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

			Program.trace("MyUDPSocket Listening on port: " + port);
		} catch (Exception ex) {
			Program.trace("MyUDPSocket Init Error: " + ex.Message);
		}
	}

	public void Listen(AsyncCallback callback) {
		// Start listening for incoming data
		_socket.BeginReceiveFrom(
			_dataStream, 0,
			_dataStream.Length,
			SocketFlags.None,
			ref _endpointSender,
			callback,
			_endpointSender
		);
	}

	internal void SendData(byte[] packetData, EndPoint destination) {
		socket.BeginSendTo(
			packetData, 0,
			packetData.Length,
			SocketFlags.None,
			destination,
			new AsyncCallback(this.OnSendDataComplete),
			destination
		);
	}

	private void OnSendDataComplete(IAsyncResult asyncResult) {
		try {
			socket.EndSend(asyncResult);
		} catch (Exception ex) {
			Program.trace("SendData Error: " + ex.Message);
		}
	}

	internal void WaitForNextData(EndPoint epClient, AsyncCallback callback) {
		socket.BeginReceiveFrom(
			dataStream, 0,
			dataStream.Length,
			SocketFlags.None,
			ref epClient,
			callback, // <------ Recursively / Async receives more data?
			epClient
		);
	}
}

// Structure to store the client information
public struct MyClient {
	public EndPoint endPoint;
	public string name;

	public MyClient(EndPoint endPoint, string name) {
		this.endPoint = endPoint;
		this.name = name;
	}
}