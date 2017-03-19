using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MyUDP {
	public class MyUDPClient {
		private int _port;
		public int port { get { return this._port; } }

		private byte[] _dataStream;
		public byte[] dataStream { get { return this._dataStream; } }

		public MyPacket packet;

		private string _host;
		public string host { get { return this._host; } }

		private IPEndPoint _endpointReceiver;
		public IPEndPoint endpointReceiver { get { return this._endpointReceiver; } }

		private IPEndPoint _endpointSender;
		public IPEndPoint endpointSender { get { return this._endpointSender; } }

		//private Socket _socketReceiver;
		//public Socket socketReceiver { get { return this._socketReceiver; } }

		private UdpClient _socketSender;
		public UdpClient socketSender { get { return this._socketSender; } }

		private bool _isConnected = false;

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
			_port = port < 0 ? MyDefaults.PORT_SERVER : port;

			IPAddress ipAddr;

			trace("Host: " + _host);
			if(!IPAddress.TryParse(_host, out ipAddr)) {
				IPAddress[] addresses = Dns.GetHostAddresses(_host);
				if (addresses.Length == 0) {
					trace("Could not start a connection to host: " + _host);
					return;
				}

				ipAddr = addresses[0];
			}

			_endpointSender = new IPEndPoint(ipAddr, _port);
			_endpointReceiver = new IPEndPoint(IPAddress.Any, 0);

			if (autoConnect) Connect();
		}
		
		public void Close() {
			_socketSender.Close();
			//_socketReceiver.Close();
		}

		///////////////////////////////////////////////////////////////////////////////

		public void Connect() {
			_isConnected = false;

			trace("_endpointReceiver: " + _endpointReceiver);
			trace("_endpointSender: " + _endpointSender);

			try {
				//_socketReceiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				_socketSender = new UdpClient(MyDefaults.PORT_CLIENT);

				//_socketReceiver.Bind(new IPEndPoint(IPAddress.Any, this._port));

				Listen();

				trace("Connecting: " + _endpointSender);
			} catch (Exception ex) {
				trace("Connection error: " + ex.ToString());
			}
		}

		private void Listen(AsyncCallback callback=null) {
			if(callback==null) callback = new AsyncCallback(OnDataReceived);

			_socketSender.BeginReceive(callback, _endpointReceiver);
			//	_dataStream, 0,
			//	_dataStream.Length,
			//	SocketFlags.None,
			//	ref _endpointReceiver,
			//	callback,
			//	_endpointReceiver
			//);
		}

		public void Send() {
			//if(!_isConnected) return;
			try { 
				trace("Attempt to send: " + packet.clientTime);
				packet.EncodeTo(_dataStream);
				_socketSender.Send(_dataStream, _dataStream.Length, _endpointSender);
			} catch(Exception ex) {
				trace("Send error: " + ex.Message);
			}
		}

		///////////////////////////////////////////////////////////////////////////////

		private void OnDataReceived(IAsyncResult asyncResult) {
			try {
				//_socketReceiver.EndReceiveFrom(asyncResult, ref _endpointReceiver);
				_socketSender.EndReceive(asyncResult, ref _endpointReceiver);
				this.packet.DecodeFrom(_dataStream);
			
				trace(_endpointReceiver + " {0} ({1})...", packet.clientTime, packet.numOfCommands);
			} catch(Exception ex) {
				trace("OnDataReceived error: " + ex.Message);
				if(ex.Message.Contains("forcibly")) {
					trace("setting connect to false......");
					_isConnected = false;
				}
			}

			Listen();
		}
	}
}
