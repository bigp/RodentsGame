using System;
using System.Threading;

namespace MyUDP { 
	static class ServerProgram {
		static ManualResetEvent _quitEvent = new ManualResetEvent(false);
		static MyUDPServer server;

		[STAThread] /// The main entry point for the application.
		static void Main() {
			Console.CancelKeyPress += (sender, eArgs) => {
				_quitEvent.Set();
				eArgs.Cancel = true;
				server.Close();
			};

			server = new MyUDPServer();
			server.SetPacketType<UnityPacket>();

			_quitEvent.WaitOne();
		}
	}
}