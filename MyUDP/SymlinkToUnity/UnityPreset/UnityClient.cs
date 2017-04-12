using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyUDP;

//using MyUDP.Packet;

namespace MyUDP.UnityPreset {
    using Clock;
    using v20;
    //using CommandsByTypes = Dictionary<EPacketTypes, Command>;

    [Flags]
    public enum EClientStatus { NEW = 1, CONNECTED = 2, SLEEPING = 4, WAKING = 8, DISCONNECTED = 16, }
    public enum EClientMessageFlow { INCOMING, OUTGOING }

    public class UnityClient {
        public static int ID_COUNTER = 0;
        public Clockwork clockTicker;
        public Client2 client;
        public UnityServer server;
        public EClientStatus status;
        public double timeDiff = 0;
        public double timeLastReceived = 0;
        public int id = 0;

        private List<Message2> _messageDiscarded;
        private MessageQueue2 _messageQueueIn;
        public MessageQueue2 messageQueueIn { get { return this._messageQueueIn; } }

        private MessageQueue2 _messageQueueOut;
        public MessageQueue2 messageQueueOut { get { return this._messageQueueOut; } }

        public static void trace(object msg) {
            Log.trace(msg);
        }

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
                client.OnReceivedBytes += OnClientReceivedBytes;
            }

            _messageQueueIn = new MessageQueue2();
            _messageQueueOut = new MessageQueue2();
            _messageDiscarded = new List<Message2>();

            //If the internal clock isn't required, anything below zero won't initialize it:
            if (clockRatePerSecond<0) return;

            //Add a client-side Master-Clock / ticker:
            clockTicker = new Clockwork().StartAutoUpdate(clockRatePerSecond);

            //Add a gear to periodically check the incoming and outgoing messages:
            clockTicker.AddGear()
                        .AddListener(__ProcessMessage)
                        .SetParams(timeMode: EGearTimeMode.FRAME_BASED);

            //Add a gear to send a "heartbeat" packet to the server, basically a *keep-alive* signal:
            clockTicker.AddGear("HeartBeat")
                        .AddListener(__HeartBeat)
                        .SetParams(counterReset: 1, timeMode:EGearTimeMode.TIME_BASED);

            //clockTicker.AddInterleaving(OnSendPosition, OnSendRotation);
        }

        private void OnClientReceivedBytes(byte[] bytesFromServer) {
            //client.packetStream
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

            WriteHeader(stream, EPacketProtoID._00_HEART_BEAT, (byte) Utils.Random(15) );
            stream.WriteStrings("Hello World");
            client.Send();
        }

        private void WriteHeader(PacketStream2 stream, EPacketProtoID protoID, byte protoParams) {
            byte protoIDOffset = (byte) ((int) protoID << 4);

            stream.ResetByteIndex();
            stream.WriteBytes(protoIDOffset | protoParams);
        }

        private void __ProcessMessage(Gear obj) {
            ProcessMessageQueue(this.messageQueueIn, EClientMessageFlow.INCOMING);
            ProcessMessageQueue(this.messageQueueOut, EClientMessageFlow.OUTGOING);
        }

        public void ProcessMessageQueue(MessageQueue2 queue, EClientMessageFlow flowType) {
            if (!queue.hasMessages) return;

            List<Message2> messages = queue.messages;
            PacketStream2 stream = client.packetStream;
            Message2 msg;

            for (int m = 0; m < messages.Count; m++) {
                msg = messages[m];

                stream.ResetByteIndex();
                msg.bytes.CopyTo(stream.byteStream, 0);

                if(ProcessMessage(stream, msg, flowType)) _messageDiscarded.Add(msg);
            }

            while(_messageDiscarded.Count>0) {
                messages.Remove(_messageDiscarded.Pop().Recycle());
            }

            _messageDiscarded.Clear();
        }

        private bool ProcessMessage(PacketStream2 stream, Message2 msg, EClientMessageFlow flowType) {
           bool canDiscard = true;
            
           MessageQueue2 outgoing = _messageQueueOut;

           switch (flowType) {
                case EClientMessageFlow.INCOMING:
                    byte protoHeader = stream.ReadByte();
                    byte protoID = (byte) (protoHeader >> 4);
                    byte protoParams = (byte) (protoHeader & 0xf);

                    EPacketProtoID PROTO_ID = (EPacketProtoID) protoID;
                    
                    switch(PROTO_ID) {
                        case EPacketProtoID._00_HEART_BEAT:
                            string heartBeatMessage = stream.ReadString();
                            trace("heartBeatMessage: " + heartBeatMessage);

                            //Ouff... I think we need another temporary Stream / Bytes to write the pending outgoing messages!!!
                            //outgoing.AddBytes()
                            break;
                        default:
                            trace("Unknown proto-ID: " + PROTO_ID);
                            break;
                    }
                    break;

                case EClientMessageFlow.OUTGOING:

                    break;
            }

            return canDiscard;
        }
    }
}
