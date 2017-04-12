using System;
using System.Threading;

#if JUST_CONSOLE
namespace MyUDP {
    using UnityPreset;
    using v20;

    class Program {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        static ConsoleKeyInfo keyInfo;

        static void Main(string[] args) {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;

                if(server!=null) server.Close();
                if(client!=null) client.Close();
            };

            Log.trace("MyUDP Menu: If you want to run the server, type 's'.");
            Log.trace("            Otherwise, let the timer run out.");

            keyInfo = Utils.ReadKey(3000);

            Log.traceClear();

            if (keyInfo.KeyChar=='s')   MainServer();
            else                        MainClient();

            _quitEvent.WaitOne();
        }

        ////////////////////////////////////////////////////////////////////////////////////

        static UnityServer server;
        static UnityClient client;

        private static void MainServer() {
            server = new UnityServer();
        }

        private static void MainClient() {
            Client2 coreClient = new Client2("127.0.0.1");
            coreClient.SetAsClientSide();

            client = new UnityClient(coreClient, 1f);
            client.clockTicker.isClearOnInternalClock = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////
    }
}

#endif