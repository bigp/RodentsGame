using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace MyUDP {
	public static class MyUDP {
		public static string[] Split(this string str, string delim) {
			return str.Split(new string[] { delim }, StringSplitOptions.None);
		}

		//public static string[] Split(this string str, char delim) {
		//	return str.Split(new char[] { delim }, StringSplitOptions.None);
		//}

        public static T Pop<T>(this List<T> list) {
            int lastID = list.Count - 1;
            T item = list[lastID];
            list.RemoveAt(lastID);
            return item;
        }

		public static string Join<T>(this T[] strArr, string delim) {
			return string.Join(delim, (string[]) (object) strArr);
		}

        public static string Times(this string str, int repeatCount) {
            string output = "";
            while (repeatCount > 0) {
                output += str;
                repeatCount--;
            }
            return output;
        }

        public static string ToHex(this byte[] bytes) {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
    }

	public static class Utils {
		public static Random randoo = new Random();

		public static Timer setTimeout(TimerCallback cb, int intervalMS = 250, bool autoRepeat=false) {
            int period = Timeout.Infinite;
            if(autoRepeat) period = intervalMS;

            Timer tmr = new Timer(cb, null, intervalMS, period);
            return tmr;
		}

		private static DateTime START_DATE_1970 = new DateTime(1970, 1, 1);
		public static ulong GetTime() {
			TimeSpan t = DateTime.Now.ToUniversalTime() - START_DATE_1970;
			return (ulong)(t.TotalMilliseconds + 0.5);
		}

		public static DateTime GetDate(ulong millis) {
			DateTime result = DateTime.SpecifyKind(START_DATE_1970, DateTimeKind.Utc);
			return result.AddMilliseconds(millis).ToLocalTime();
		}

		public static string Random(string[] names) {
			return names[randoo.Next(names.Length)];
		}

		public static int Random(int min, int max) {
			return randoo.Next(min, max);
		}

		public static int Random(int range) {
			return randoo.Next(range);
		}

		public static void ForEachFlags<T>(T selected, Action<T> cbForEach) {
            int selectedUint = (int)(object)selected;
			foreach (T type in Enum.GetValues(typeof(T))) {
				if ((selectedUint & ((int) (object) type)) > 0 ) cbForEach(type);
			}
		}

		public static bool stringIsOK(string str) {
			return !string.IsNullOrEmpty(str) && str.Trim().Length > 0;
		}

#if JUST_CONSOLE
        delegate ConsoleKeyInfo ReadKeyInfo();

        public static ConsoleKeyInfo ReadKey(int timeoutms, char defaultChar = ' ', ConsoleKey defaultKey = ConsoleKey.Spacebar) {
            ReadKeyInfo d = Console.ReadKey;
            IAsyncResult result = d.BeginInvoke(null, null);
            result.AsyncWaitHandle.WaitOne(timeoutms);

            if (result.IsCompleted) {
                ConsoleKeyInfo keyInfo = d.EndInvoke(result);
                Console.WriteLine("Key: " + keyInfo);
                return keyInfo;
            } else {
                Console.WriteLine("Timed out!");
                return new ConsoleKeyInfo(defaultChar, defaultKey, false, false, false);
            }
        }
#endif
    }

	public static class Log {
#if JUST_CONSOLE
        public static void traceClear() {
            Console.Clear();
        }

        public static void trace(object a, params object[] args) {
			Console.WriteLine(a == null ? "*null*" : a.ToString(), args);
		}

		public static void traceError(object a, params object[] args) {
			ConsoleColor before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			trace(a, args);
			Console.ForegroundColor = before;
		}

        //////////////////// Use a **Buffer** for "ClearScreen" print outs:

        private static string _current = "";

        public static void BufferClear() {
            _current = "";
        }
        public static void BufferLine(object a, params object[] args) {
            _current += string.Format(a == null ? "*null*" : a.ToString(), args) + "\n";
        }

        public static void BufferString(object a, params object[] args) {
            _current += string.Format(a == null ? "*null*" : a.ToString(), args);
        }

        public static void BufferOutput() {
            if(string.IsNullOrEmpty(_current)) return;
            trace(_current);
        }
#else
        public static void traceClear() {
            //UnityEngine.Debug.ClearDeveloperConsole();
        }

		public static void trace(object a, params object[] args) {
            UnityEngine.Debug.Log(a == null ? "*null*" : a.ToString());
		}

		public static void traceError(object a, params object[] args) {
            UnityEngine.Debug.LogError(a);
		}

        public static void BufferClear() {}
        public static void BufferLine(object a, params object[] args) {}
        public static void BufferString(object a, params object[] args) {}
        public static void BufferOutput() {}
#endif
    }

    public static class MyDefaults {
		//////////////////////////////////////////////////////////// Constants & Defaults:
		public static string CLIENT_IP = "192.168.2.127";
		public static int CLIENT_PORT = 11000;
		public static int SERVER_PORT = 11000;
		public static int DATA_STREAM_SIZE = 1024;
	}
}
