using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyUDP { 
	static class Program {
		static ManualResetEvent _quitEvent = new ManualResetEvent(false);
		static MyUDPServer server;

		[STAThread] /// The main entry point for the application.
		static void Main() {
			Console.CancelKeyPress += (sender, eArgs) => {
				_quitEvent.Set();
				eArgs.Cancel = true;
			};

			server = new MyUDPServer();

			Utils.setTimeout((object state) => {
				Log.trace("Hello World!");
			});
			_quitEvent.WaitOne();
		}
	}

	static class Log {
		public static void trace(object a, params object[] args) {
			Console.WriteLine(a==null ? "*null*" : a.ToString(), args);
		}
	}

	static class Utils {
		public static Timer setTimeout(TimerCallback cb, int intervalMS=250) {
			Timer tmr = new Timer(cb, null, intervalMS, Timeout.Infinite );
			return tmr;
		}
	}
}