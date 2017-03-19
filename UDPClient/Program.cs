using System;
using System.Threading;

namespace MyUDP {
	class ProgramClient {

		static ManualResetEvent _quitEvent = new ManualResetEvent(false);
		static MyUDPClient client;

		[STAThread] /// The main entry point for the application.
		static void Main() {
			Log.trace("Started Client!");

			Console.CancelKeyPress += (sender, eArgs) => {
				_quitEvent.Set();
				eArgs.Cancel = true;
			};

			client = new MyUDPClient();

			//Utils.setTimeout((object state) => {
			//	Log.trace("Hello World!");
			//});

			_quitEvent.WaitOne();
		}
	}

	
}
