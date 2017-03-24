using System;
using System.Threading;

#if JUST_CONSOLE
namespace MyUDP {
    using Clock;
    using UnityPreset;

    class Program {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        static UnityServer server;
        static MyUDPClient client;
        static Clockwork clock;

        static void Main(string[] args) {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;

                if(server!=null) server.Close();
            };

            Log.trace("MyUDP Menu: If you want to run the server, type 's'.");
            Log.trace("            Otherwise, press ENTER to run the client.");

            ConsoleKeyInfo keyInfo = Console.ReadKey();

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
            server = new UnityServer();
            //server.OnDataReceived += (MyUDPPacket packet, MyUDPServerClient client) => {
            //    Log.trace("GOT DATA!");
            //};
        }
        private static void MainClient() {
            client = new MyUDPClient();
            
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////


        private static void Main2() {
            clock = new Clockwork().StartAutoUpdate(2);
            clock.AddListener(OnClockTick, false);
            clock.AddInterleaving(OnChildGearTick1, OnChildGearTick2);
            clock.AddInterleaving(OnChildGearTick3, OnChildGearTick3, OnChildGearTick3);
        }

        private static void OnChildGearTick1(Gear gear) {
            Log.trace("------------- Called 1: " + gear.name);
        }

        private static void OnChildGearTick2(Gear gear) {
            Log.trace("------------- Called 2: " + gear.name);
        }

        private static void OnChildGearTick3(Gear gear) {
            Log.trace("------------- Called ... " + gear.name);
        }

        private static void OnClockTick(Gear masterClock) {
            Log.BufferClear();
            Log.BufferAdd("------- " + masterClock.name + " : " + Utils.GetTime());
        }
    }
}

#endif