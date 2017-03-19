using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MyUDP {
	public class MyUDPClient {
		private UdpClient _socket;

		private int _port;
		public int port { get { return this._port; } }

		private byte[] _dataStream;
		public byte[] dataStream { get { return this._dataStream; } }

		public MyPacket packet;

		private string _host;
		public string host { get { return this._host; } }

		/////////////////////////////////////////////////////////////////////////////// Internal helper methods:

		public static void trace(object obj, params object[] args) {
			ConsoleColor before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("[MyUDP Client] ");
			Console.ForegroundColor = before;

			Log.trace(obj.ToString(), args);
		}

		/////////////////////////////////////////////////////////////////////////////// Constructor:

		public MyUDPClient(string hostname = "127.0.0.1", int port = -1, int dataStreamSize = -1, bool autoConnect = true) {
			if (dataStreamSize < 0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;

			packet = new MyPacket();

			_dataStream = new byte[dataStreamSize];
			_host = hostname;
			_port = port < 0 ? MyDefaults.PORT : port;
			_socket = new UdpClient(port);

			if (autoConnect) Connect();
		}

		private void Connect() {
			try { 
				_socket.Connect(this._host, this._port);


				/////////////////////////////////////// vvvv ?????
				Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");

				_socket.Send(sendBytes, sendBytes.Length);

				// Sends a message to a different host using optional hostname and port parameters.
				UdpClient udpClientB = new UdpClient();
				udpClientB.Send(sendBytes, sendBytes.Length, "AlternateHostMachineName", 11000);

				//IPEndPoint object will allow us to read datagrams sent from any source.
				IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

				// Blocks until a message returns on this socket from a remote host.
				Byte[] receiveBytes = _socket.Receive(ref RemoteIpEndPoint);
				string returnData = Encoding.ASCII.GetString(receiveBytes);

				// Uses the IPEndPoint object to determine which of these two hosts responded.
				Console.WriteLine("This is the message you received " +
											 returnData.ToString());
				Console.WriteLine("This message was sent from " +
											RemoteIpEndPoint.Address.ToString() +
											" on their port number " +
											RemoteIpEndPoint.Port.ToString());

				_socket.Close();
				udpClientB.Close();
			} catch (Exception ex) {
				trace("Connection error: " + ex.ToString());
			}
		}
	}
}
