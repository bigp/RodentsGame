//using System;
//using System.Net;

//namespace MyUDP.Core {
//	public abstract class MyUDPCore {
//		internal int _port;
//		public int port { get { return this._port; } }

//		internal byte[] _byteStream;
//		public byte[] byteStream { get { return this._byteStream; } }

//		internal IPEndPoint _endpointIn;
//		public IPEndPoint endpointIn { get { return this._endpointIn; } }

//		protected MyUDPPacket _packet;
//		public MyUDPPacket packet { get { return this._packet; } }
		
//		public MyUDPCore(int port, int dataStreamSize) {
//			if (dataStreamSize < 0) dataStreamSize = MyDefaults.DATA_STREAM_SIZE;
//			_port = port < 0 ? MyDefaults.SERVER_PORT : port;
			
//			_byteStream = new byte[dataStreamSize];
//			_endpointIn = new IPEndPoint(IPAddress.Any, 0);
//		}

//		public void SetPacketType<PKT>() where PKT : MyUDPPacket, new() {
//			_packet = new PKT();
//		}

//		public void SetPacketType(Type type) {
//			_packet = (MyUDPPacket)Activator.CreateInstance(type);
//		}
//	}
//}
