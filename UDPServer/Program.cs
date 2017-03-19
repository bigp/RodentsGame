using System;
using System.Threading;

namespace MyUDP { 
	static class ProgramServer {
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

			//Utils.setTimeout((object state) => {
			//	Log.trace("Hello World!");
			//});

			_quitEvent.WaitOne();
		}
	}

	
}