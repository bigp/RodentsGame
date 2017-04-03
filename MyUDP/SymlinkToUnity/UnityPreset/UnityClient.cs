using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using MyUDP.Packet;

namespace MyUDP.UnityPreset {
    using Clock;
    using Rev2Beta;
    //using CommandsByTypes = Dictionary<EPacketTypes, Command>;

    [Flags]
    public enum EClientStatus {
        NEW = 1,
        CONNECTED = 2,
        SLEEPING = 4,
        WAKING = 8,
        DISCONNECTED = 16,
    }

    public class UnityClient {
        public static int ID_COUNTER = 0;
        public Clockwork clockTicker;
        public Client2 client;
        public EClientStatus status;
        public double timeDiff = 0;
        public double timeLastReceived = 0;
        public int id = 0;
        

        private MessageQueue2 _messageQueueIn;
        public MessageQueue2 messageQueueIn { get { return this._messageQueueIn; } }

        private MessageQueue2 _messageQueueOut;
        public MessageQueue2 messageQueueOut { get { return this._messageQueueOut; } }


        public bool isServerSide { get { return client.isServerSide; } }
        public bool HasStatus( EClientStatus whichStatus ) {
            return (status & whichStatus)>0;
        }

        ////////////////////////////////////////////////////////////////////////////////////

        public UnityClient(Client2 client=null, float clockRatePerSecond = 2f) {
            this.client = client;
            this.status = EClientStatus.NEW;
            this.id = ID_COUNTER++;

            if(client!=null) {
                client.OnClientReceivedBytes += OnClientReceivedBytes;
            }
            _messageQueueIn = new MessageQueue2();
            _messageQueueOut = new MessageQueue2();

            //If the internal clock isn't required, anything below zero won't initialize it:
            if (clockRatePerSecond<0) return;

            clockTicker = new Clockwork().StartAutoUpdate(clockRatePerSecond);
            clockTicker.AddGear().AddListener(__ProcessMessage).SetParams(timeMode: EGearTimeMode.FRAME_BASED);

            clockTicker.AddGear("HeartBeat").AddListener(__HeartBeat).SetParams(counterReset: 1, timeMode:EGearTimeMode.TIME_BASED);
            //clockTicker.AddInterleaving(OnSendPosition, OnSendRotation);
        }

        private void OnClientReceivedBytes(byte[] bytesFromServer) {
            _messageQueueIn.AddBytes(bytesFromServer);
        }

        public void Close() {
            if(client!=null) client.Close();
            clockTicker.Dispose();
        }

        ////////////////////////////////////////////////////////////////////////////////////

        private void __HeartBeat(Gear obj) {
            Log.traceClear();
            Log.trace(Utils.GetTime());

            PacketStream2 stream = client.packetStream;
            stream.ResetByteIndex();
            stream.WriteBytes(0x01);
            client.Send();
        }

        private void __ProcessMessage(Gear obj) {
            if (this.messageQueueIn.hasMessages) ProcessMessagesIn(this.messageQueueIn);
            if (this.messageQueueOut.hasMessages) ProcessMessagesOut(this.messageQueueOut);
        }

        public void ProcessMessagesIn(MessageQueue2 queue) {
            Log.trace("Client Processing IN: " + queue.messages.Count);
        }

        public void ProcessMessagesOut(MessageQueue2 queue) {
            Log.trace("Client Processing OUT: " + queue.messages.Count);
        }
    }
}
