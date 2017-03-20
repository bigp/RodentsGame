using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MyUDP {
	using Packet;

	public class MyUDPClient {
		private static string[] RANDOM_NAMES = new string[] { "bob", "fred", "andy", "frank", "john", "julia", "barb", "simone", "tiffany", "sarah"};
		private int _port;
		public int port { get { return this._port; } }

		private byte[] _dataStream;
		public byte[] dataStream { get { return this._dataStream; } }

		public MyPacket packet;
		public string randomName;

		private string _host;
		public string host { get { return this._host; } }

		private IPEndPoint _endpointIn;
		public IPEndPoint endpointIn { get { return this._endpointIn; } }

		private IPEndPoint _endpointOut;
		public IPEndPoint endpointOut { get { return this._endpointOut; } }

		private UdpClient _socketOut;
		public UdpClient socketOut { get { return this._socketOut; } }

		//private UdpClient _socketIn;
		//public UdpClient socketIn { get { return this._socketIn; } }

		//private bool _isConnected = false;

		/////////////////////////////////////////////////////////////////////////////// Internal helper methods:

		public static void trace(object obj, params object[] args) {
			ConsoleColor before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("[MyUDP Client] ");
			Console.ForegroundColor = before;

			Log.trace(obj.ToString(), args);
		}

		public static void traceError(object obj, params object[] args) {
			ConsoleColor before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			trace(obj, args);
			Console.ForegroundColor = before;
		}

		/////////////////////////////////////////////////////////////////////////////// Constructor:

		public MyUDPClient(string hostname = "127.0.0.1", int port = -1, int dataStreamSize = -1, bool autoConnect = true) {
			if (dataStreamSize < 0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;
			
			packet = new MyPacket();
			randomName = Utils.Random(RANDOM_NAMES) + Utils.Random(10, 99);
			_dataStream = new byte[dataStreamSize];
			_host = hostname;
			_port = port < 0 ? MyDefaults.SERVER_PORT : port;

			trace("THIS IS = " + randomName);

			IPAddress ipAddr;

			if(!IPAddress.TryParse(_host, out ipAddr)) {
				IPAddress[] addresses = Dns.GetHostAddresses(_host);
				if (addresses.Length == 0) {
					traceError("Could not start a connection to host: " + _host);
					return;
				}

				ipAddr = addresses[0];
			}

			_endpointOut = new IPEndPoint(ipAddr, _port);
			_endpointIn = new IPEndPoint(IPAddress.Any, 0);

			if (autoConnect) Connect();
		}
		
		public void Close() {
			_socketOut.Close();
		}

		///////////////////////////////////////////////////////////////////////////////

		public void Connect(int port = -1) {
			if(port<0) port = this._port;

			try {
				_socketOut = new UdpClient(port);
			} catch (Exception ex) {
				//If it's a blocked port, try the next one:
				if(ex.Message.Contains("Only one usage")) {
					Connect(port + 1);
					return;
				} else {
					traceError("Connection ERROR! ------ " + ex.Message);
				}
			}

			if(_socketOut!=null) Listen();
		}

		private void Listen(AsyncCallback callback=null) {
			if(callback==null) callback = OnDataReceived;

			_socketOut.BeginReceive(callback, _endpointIn);
		}

		public void Send() {
			try {
				packet.EncodeTo(_dataStream);
				_socketOut.Send(_dataStream, packet.byteLength, _endpointOut);
			} catch(Exception ex) {
				traceError("Send error: " + ex.Message);
			}
		}

		///////////////////////////////////////////////////////////////////////////////

		private void OnDataReceived(IAsyncResult asyncResult) {
			MyPacket packetReceived = new MyPacket();

			try {
				byte[] bytesReceived = _socketOut.EndReceive(asyncResult, ref _endpointIn);
				packetReceived.DecodeFrom(bytesReceived);
			
				trace("{0} ({1}) jsonData: {2}", packetReceived.clientTimeFormatted, packetReceived.numOfCommands, packetReceived.commands[0].jsonData);
			} catch(Exception ex) {
				traceError("OnDataReceived error: " + ex.Message);
				if(ex.Message.Contains("forcibly")) {
					traceError("setting connect to false......");
					//_isConnected = false;
				}
			}

			Listen();
		}
	}
}
