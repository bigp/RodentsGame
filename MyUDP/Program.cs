using System;
using System.Threading;

#if JUST_CONSOLE
namespace MyUDP {
    using Clock;

    class Program {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        //static MyUDPServer server;
        static Clockwork clock;

        static void Main(string[] args) {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
                //server.Close();
            };

            //server = new MyUDPServer();
            //server.SetPacketType<UnityPacket>();
            clock = new Clockwork().StartAutoUpdate(2);
            clock.AddListener(OnClockTick, false);
            //clock.AddGear("Child Gear 1").AddListeners(OnChildGearTick1, OnChildGearTick2).SetParams(5, true, EGearTimeMode.FRAME_BASED);
            //clock.AddGear("Child Gear 2").AddListener(OnChildGearTick3).SetParams();
            clock.AddInterleaving(OnChildGearTick1, OnChildGearTick2);
            clock.AddInterleaving(OnChildGearTick3, OnChildGearTick3, OnChildGearTick3);
            _quitEvent.WaitOne();

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