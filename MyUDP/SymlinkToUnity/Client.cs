//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Net;
//using System.Net.Sockets;

//namespace MyUDP {
//	//using Packet;
//	using Core;
	
//	public class MyUDPClient : MyUDPCore {
//		private static string[] RANDOM_NAMES = new string[] { "bob", "fred", "andy", "frank", "john", "julia", "barb", "simone", "tiffany", "sarah"};
		
//		private string _host;
//		public string host { get { return this._host; } }

//		private IPEndPoint _endpointOut;
//		public IPEndPoint endpointOut { get { return this._endpointOut; } }

//		private UdpClient _socket;
//		public UdpClient socket { get { return this._socket; } }

//		//private bool _isConnected = false;

//		////////////////

//		public Action<MyUDPPacket> OnPacketReceived;
//		public Action<MyUDPPacket> OnPacketPreSend;

//		/////////////////////////////////////////////////////////////////////////////// Internal helper methods:

//#if !UNITY_EDITOR
//		public static void trace(object obj, params object[] args) {
//			ConsoleColor before = Console.ForegroundColor;
//			Console.ForegroundColor = ConsoleColor.Cyan;
//			Console.Write("[MyUDP Client] ");
//			Console.ForegroundColor = before;

//			Log.trace(obj.ToString(), args);
//		}

//		public static void traceError(object obj, params object[] args) {
//			ConsoleColor before = Console.ForegroundColor;
//			Console.ForegroundColor = ConsoleColor.Red;
//			trace(obj, args);
//			Console.ForegroundColor = before;
//		}
//#else
//		public static void trace(object obj, params object[] args) {
//			Log.trace("[MyUDP Client] " + obj.ToString(), args);
//		}

//		public static void traceError(object obj, params object[] args) {
//			Log.traceError("[MyUDP Client] ERROR: " + obj.ToString(), args);
//		}
//#endif
//		/////////////////////////////////////////////////////////////////////////////// Constructor:

//		public MyUDPClient(string hostname = "127.0.0.1", int port = -1, int dataStreamSize = -1, bool autoConnect=true) : base(port, dataStreamSize) {
//			_host = hostname;
			
//			IPAddress ipAddr;

//			if (!IPAddress.TryParse(_host, out ipAddr)) {
//				IPAddress[] addresses = Dns.GetHostAddresses(_host);
//				if (addresses.Length == 0) {
//					traceError("Cannot resolve the host: " + _host);
//					return;
//				}

//				ipAddr = addresses[0];
//			}

//			_endpointOut = new IPEndPoint(ipAddr, _port);

//			if(autoConnect) Connect(_port);
//		}

//        public virtual void Close() {
//			_socket.Close();
//		}

//		///////////////////////////////////////////////////////////////////////////////

//		public virtual void Connect(int port = -1) {
//			if(port<0) port = this._port;

//			try {
//				_socket = new UdpClient(port);
//			} catch (Exception ex) {
//				//If it's a blocked port, try the next one:
//				if(ex.Message.Contains("Only one usage")) {
//					Connect(port + 1);
//					return;
//				} else {
//					traceError("Connection ERROR! ------ " + ex.Message);
//				}
//			}

//			if(_socket!=null) Listen();
//		}

//		private void Listen(AsyncCallback callback=null) {
//			if(callback==null) callback = OnDataReceived;

//			_socket.BeginReceive(callback, _endpointIn);
//		}

//		public virtual void Send() {
//			try {
//				_packet.Encode(_byteStream);
//				if (OnPacketPreSend != null) OnPacketPreSend(_packet);

//				_socket.Send(_byteStream, _packet.byteLength, _endpointOut);
//			} catch (Exception ex) {
//				traceError("Send error: " + ex.ToString());
//			}
//		}


//		///////////////////////////////////////////////////////////////////////////////

//		private void OnDataReceived(IAsyncResult asyncResult) {
//			try {
//				byte[] bytesReceived = _socket.EndReceive(asyncResult, ref _endpointIn);
//				_packet.Decode(bytesReceived);
//				if(OnPacketReceived!=null) OnPacketReceived(_packet);

//			} catch(Exception ex) {
//				traceError("OnDataReceived error: " + ex.Message);
//				if(ex.Message.Contains("forcibly")) {
//					//_isConnected = false;
//				}
//			}

//			Listen();
//		}
//	}
//}
