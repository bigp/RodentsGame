using System;
using System.Threading;

namespace MyUDP {
	public static class Utils {
		public static Timer setTimeout(TimerCallback cb, int intervalMS = 250) {
			Timer tmr = new Timer(cb, null, intervalMS, Timeout.Infinite);
			return tmr;
		}

		private static DateTime START_DATE = new DateTime(1970, 1, 1);
		public static ulong GetTime() {
			TimeSpan t = DateTime.Now.ToUniversalTime() - START_DATE;
			return (ulong)(t.TotalMilliseconds + 0.5);
		}
	}

	public static class Log {
		public static void trace(object a, params object[] args) {
			Console.WriteLine(a == null ? "*null*" : a.ToString(), args);
		}
	}

	public static class MyDefaults {
		/////////////////////////////////////////////////////////////////////////////// Constants & Defaults:
		public static int DATA_STREAM_SIZE = 1024;
		public static int PORT_SERVER = 11000;
		public static int PORT_CLIENT = 11001;


	}
}
