using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyUDP.Packet;

namespace MyUDP.UnityPreset {
    using Clock;
    using CommandsByTypes = Dictionary<EPacketTypes, Command>;

    public class UnityClient : MyUDPClient {
        public Clockwork clientClock;
        public new UnityPacket packet { get { return (UnityPacket) this._packet; } }
        public List<Command> commandsNotAcked;
        private CommandsByTypes commandsLast;

        private int _ackCounter = 1;

        public UnityClient(float clockRatePerSecond= 10f, string hostname = "127.0.0.1", int port = -1, int dataStreamSize = -1, bool autoConnect = true) : base(hostname, port, dataStreamSize, autoConnect) {
            commandsNotAcked = new List<Command>();
            commandsLast = new CommandsByTypes();
            SetPacketType<UnityPacket>();

            OnPacketReceived += OnUnityPacketReceived;
            clientClock = new Clockwork().StartAutoUpdate(10);
            clientClock.AddGear().AddListener(OnCommandClear).SetParams(timeMode: EGearTimeMode.FRAME_BASED);
            clientClock.AddInterleaving(OnSendPosition, OnSendRotation);
        }

        private void OnUnityPacketReceived(MyUDPPacket incomingPacket) {
            trace("Packet received: " + packet.commands.Count);
            foreach(Command cmd in packet.commands) {
                //cmd.ac
            }

            throw new NotImplementedException();
        }

        public override void Close() {
            base.Close();
            clientClock.Dispose();
        }

        public void OnCommandClear(Gear gear) {
            packet.commands.Clear();
        }

        public Command CreateCommand(EPacketTypes type) {
            if (commandsLast.ContainsKey(type)) {
                packet.commands.Add(commandsLast[type]);
            }
            Command cmd = new Command();
            cmd.ackID = _ackCounter++;
            cmd.types |= type;
            packet.clientTime = Utils.GetTime();
            packet.commands.Add(cmd);
            commandsLast[type] = cmd;
            return cmd;
        }

        public void OnSendPosition(Gear gear) {
            Command cmd = CreateCommand(EPacketTypes.POSITION);
            cmd.xyzData.position[0] = Utils.randoo.NextDouble();
            cmd.xyzData.position[1] = Utils.randoo.NextDouble();
            cmd.xyzData.position[2] = Utils.randoo.NextDouble();
            //trace("Positions: " + packet.commands.Count);
            Send();
        }

        public void OnSendRotation(Gear gear) {
            Command cmd = CreateCommand(EPacketTypes.ROTATION);
            cmd.xyzData.rotation[0] = Utils.randoo.NextDouble();
            cmd.xyzData.rotation[1] = Utils.randoo.NextDouble();
            cmd.xyzData.rotation[2] = Utils.randoo.NextDouble();
            cmd.xyzData.rotation[3] = Utils.randoo.NextDouble();
            //trace("Rotations: " + packet.commands.Count);
            Send();
        }
    }
}
