using System;
using System.Threading;

namespace MyUDP {
	public static class MyUDP {
		public static string[] Split(this string str, string delim) {
			return str.Split(new string[] { delim }, StringSplitOptions.None);
		}

		public static string[] Split(this string str, char delim) {
			return str.Split(new char[] { delim }, StringSplitOptions.None);
		}
	}

	public static class Utils {
		public static Random randy = new Random();

		public static Timer setTimeout(TimerCallback cb, int intervalMS = 250) {
			Timer tmr = new Timer(cb, null, intervalMS, Timeout.Infinite);
			return tmr;
		}

		private static DateTime START_DATE = new DateTime(1970, 1, 1);
		public static ulong GetTime() {
			TimeSpan t = DateTime.Now.ToUniversalTime() - START_DATE;
			return (ulong)(t.TotalMilliseconds + 0.5);
		}

		public static DateTime GetDate(ulong millis) {
			DateTime result = DateTime.SpecifyKind(START_DATE, DateTimeKind.Utc);
			return result.AddMilliseconds(millis).ToLocalTime();
		}

		public static string Random(string[] names) {
			return names[randy.Next(names.Length)];
		}

		public static int Random(int min, int max) {
			return randy.Next(min, max);
		}

		public static int Random(int range) {
			return randy.Next(range);
		}

		public static void ForEachFlags<T>(T selected, Action<T> cbForEach) {
			foreach (T type in Enum.GetValues(typeof(T))) {
				if (((Enum)(object)selected).HasFlag((Enum) (object) type)) cbForEach(type);
			}
		}

		public static bool stringIsOK(string str) {
			return !string.IsNullOrEmpty(str) && str.Trim().Length > 0;
		}
	}

	public static class Log {
		public static void trace(object a, params object[] args) {
			Console.WriteLine(a == null ? "*null*" : a.ToString(), args);
		}

		public static void traceError(object a, params object[] args) {
			ConsoleColor before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			trace(a, args);
			Console.ForegroundColor = before;
		}
	}

	public static class MyDefaults {
		//////////////////////////////////////////////////////////// Constants & Defaults:
		public static string CLIENT_IP = "192.168.2.127";
		public static int CLIENT_PORT = 11000;
		public static int SERVER_PORT = 11000;
		public static int DATA_STREAM_SIZE = 1024;
	}
}
