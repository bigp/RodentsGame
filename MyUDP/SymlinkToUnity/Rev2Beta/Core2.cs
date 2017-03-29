using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MyUDP.Rev2Beta {
    public class Core2 {
        internal int _port;
        public int port { get { return this._port; } }

        internal IPEndPoint _endpointIn;
        public IPEndPoint endpointIn { get { return this._endpointIn; } }
        
        public Core2(int port) {
            _port = port < 0 ? MyDefaults.SERVER_PORT : port;

            _endpointIn = new IPEndPoint(IPAddress.Any, 0);
        }

#if JUST_CONSOLE
        public static void trace(object obj, params object[] args) {
            ConsoleColor before = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[MyUDP] ");
            Console.ForegroundColor = before;
            Log.trace(obj.ToString(), args);
        }

        public static void traceError(object obj, params object[] args) {
            ConsoleColor before = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[MyUDP] ");
            Console.ForegroundColor = before;
            Log.traceError(obj.ToString(), args);
        }
#else
		public static void trace(object obj, params object[] args) {
			//Log.trace("[MyUDP Server] " + obj.ToString(), args);
		}

		public static void traceError(object obj, params object[] args) {
			//Log.trace("[MyUDP Server] ERROR: " + obj.ToString(), args);
		}
#endif

    }
}
