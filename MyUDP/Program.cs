using System;
using System.Threading;

#if JUST_CONSOLE
namespace MyUDP {
    using Clock;
    using UnityPreset;

    class Program {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        static UnityServer server;
        static UnityClient client;

        static ConsoleKeyInfo ReadKey(int timeoutms, char defaultChar=' ', ConsoleKey defaultKey=ConsoleKey.Spacebar) {
            ReadKeyInfo d = Console.ReadKey;
            IAsyncResult result = d.BeginInvoke(null, null);
            result.AsyncWaitHandle.WaitOne(timeoutms);//timeout e.g. 15000 for 15 secs
            if (result.IsCompleted) {
                ConsoleKeyInfo keyInfo = d.EndInvoke(result);
                Console.WriteLine("Key: " + keyInfo);
                return keyInfo;
            } else {
                Console.WriteLine("Timed out!");
                return new ConsoleKeyInfo(defaultChar, defaultKey, false, false, false);
            }
        }

        delegate ConsoleKeyInfo ReadKeyInfo();

        static void Main(string[] args) {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;

                if(server!=null) server.Close();
            };

            Log.trace("MyUDP Menu: If you want to run the server, type 's'.");
            Log.trace("            Otherwise, let the timer run out: (3 seconds).");
            
            ConsoleKeyInfo keyInfo = ReadKey(3000);

            Log.traceClear();

            if (keyInfo.KeyChar=='s') {
                MainServer();
            } else {
                MainClient();
            }

            _quitEvent.WaitOne();
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////

        private static void MainServer() {
            server = new UnityServer(5);
            //server.OnDataReceived += (MyUDPPacket packet, MyUDPServerClient client) => {
            //    Log.trace("GOT DATA!");
            //};
        }

        private static void MainClient() {
            client = new UnityClient(5, "127.0.0.1");


        }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
    }
}

#endif