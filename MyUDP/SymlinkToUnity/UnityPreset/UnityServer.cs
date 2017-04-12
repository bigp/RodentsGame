using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyUDP.UnityPreset {
    using v20;
    using Clock;
    using UnityClients = Dictionary<v20.Client2, UnityClient>;
    using System.Net;

    public class UnityServer : Server2 {
        private Object thisLock = new Object();

        public Clockwork clockTicker;

        public UnityClients clientsUnity;
        public List<UnityClient> clientsToForget;

        public float timeForSleep = 8;
        public float timeForForget = 15;
        public DateTime timeStarted;

        public Action<UnityClient> OnStatusSleeping;
        public Action<UnityClient> OnStatusWaking;
        public Action<UnityClient> OnStatusReturned;
        public Action<UnityClient> OnStatusDisconnected;

        public PacketStream2 packetStreamTemp;
        
        public double GetTimeSinceStart() {
            DateTime timeNow = DateTime.Now;
            TimeSpan timeDiff = timeNow - timeStarted;
            return timeDiff.TotalMilliseconds;
        }

        public UnityServer(float ticksPerSecond=10f, int port = -1, int dataStreamSize = -1, bool autoListens = true)
                : base(port, dataStreamSize, autoListens) {

            clientsUnity = new UnityClients();
            clientsToForget = new List<UnityClient>();
            packetStreamTemp = new PacketStream2();

            timeStarted = DateTime.Now;

            //Hook-up the events:
            this.OnNewClient += OnNewUnityClient;
            this.OnReceivedFromClient += OnClientReceivedData;

            //Set the "ticker" for sending messages back at given intervals:
            clockTicker = new Clockwork().StartAutoUpdate(ticksPerSecond);
            clockTicker.AddListener(__ProcessClientMessages);
            clockTicker.AddGear("Unity Housekeeping").AddListener(__OnCheckClientsAlive);
        }

        private UnityClient Resolve(Client2 client) {
            if(!clientsUnity.ContainsKey(client)) {
                throw new Exception("Resolve ERROR - Missing 'UnityServerClient' object for client: " + client.ToString());
            }

            return clientsUnity[client];
        }

        private void OnNewUnityClient(Client2 client) {
            if(clientsUnity.ContainsKey(client)) {
                throw new Exception("OnNewUnityClient ERROR - Already has the client, NOT NEW: " + client.ToString());
            }
            
            //Pass '-1' to the clockRate to indicate we will manually control the Message flow with a few server binding hooks:
            UnityClient unityClient = new UnityClient(client, -1);
            unityClient.server = this;
            clientsUnity[client] = unityClient;
        }

        private void OnClientReceivedData(Client2 client) {
            UnityClient unityClient = Resolve(client);

            trace(client.endpointIn + ": <<<<< RECEIVED: " + _receivedBytesLength);

            //If was 'SLEEPING' (partially timed-out), set it's status to WAKING:
            if (unityClient.HasStatus(EClientStatus.SLEEPING)) {
                unityClient.status &= ~EClientStatus.SLEEPING;
                unityClient.status |= EClientStatus.WAKING;

                if (OnStatusWaking != null)
                    OnStatusWaking(unityClient);

            } else if (unityClient.HasStatus(EClientStatus.WAKING)) {
                unityClient.status &= ~EClientStatus.WAKING;

                if (OnStatusReturned != null)
                    OnStatusReturned(unityClient);

            }

            //Alright, set the client's status and time-received stamp:
            double timeNow = GetTimeSinceStart();
            unityClient.timeDiff = timeNow - unityClient.timeLastReceived;
            unityClient.timeLastReceived = timeNow;
            unityClient.status |= EClientStatus.CONNECTED;
        }

        private void __OnCheckClientsAlive(Gear obj) {
            int debugIndentCounter = 0;
            double timeNow = GetTimeSinceStart();

            UnityClients.ValueCollection clients = clientsUnity.Values;
            foreach (UnityClient unityClient in clients) {
                double diffNow = timeNow - unityClient.timeLastReceived;
                double diffSeconds = diffNow * 0.001f;
                double diffFromForget = timeForForget - diffSeconds;
                int diffInt = (int)(diffFromForget);
                int restInt = (int)(timeForForget - diffInt);

                string cliStr = unityClient.id.ToString();
                //Log.BufferString(unityClient.id + ": " + "#".Times(diffInt) + " ".Times(restInt) + " ".Times(3 - cliStr.Length));

                debugIndentCounter++;
                if((debugIndentCounter%4)==0) Log.BufferString("\n");

                if (!unityClient.HasStatus(EClientStatus.SLEEPING)) {
                    if (diffSeconds > timeForSleep) {
                        unityClient.status |= EClientStatus.SLEEPING;
                        if(OnStatusSleeping!=null) OnStatusSleeping(unityClient);
                    }
                } else if(!unityClient.HasStatus(EClientStatus.DISCONNECTED)) {
                    if(diffSeconds > timeForForget) {
                        unityClient.status |= EClientStatus.DISCONNECTED;
                        if (clientsToForget.Contains(unityClient)) {
                            throw new Exception("OnUnityCheckClientsAlive ERROR - clientsToForget already has this client: " + unityClient.ToString());
                        }
                        clientsToForget.Add(unityClient);
                    }
                }
            }

            //If there are any clients to forget, clear them out of the client-lists (Unity-based AND internal one):
            if(clientsToForget.Count>0) {
                foreach (UnityClient unityClient in clientsToForget) {
                    clientsUnity.Remove(unityClient.client);
                    if(OnStatusDisconnected!=null) OnStatusDisconnected(unityClient);
                    ForgetClient(unityClient.client.endpointIn);
                }

                clientsToForget.Clear();
            }
        }

        private void __ProcessClientMessages(Gear obj) {
            UnityClients.ValueCollection clients = clientsUnity.Values;

            foreach (UnityClient unityClient in clients) {
                Client2 client = unityClient.client;

                unityClient.ProcessMessageQueue(unityClient.messageQueueIn, EClientMessageFlow.INCOMING);
            }

            foreach (UnityClient unityClient in clients) {
                Client2 client = unityClient.client;

                testServerSendPacket(client);

                //unityClient.ProcessMessageQueue(unityClient.messageQueueOut, EClientMessageFlow.OUTGOING);

                SendData(packetStreamTemp, client);
            }
        }

        private void testServerSendPacket(Client2 client) {
            /////////////// TEST SENDING BACK SOME BYTES BACK TO CLIENTS!
            packetStreamTemp.ResetByteIndex();
            packetStreamTemp.WriteBytes((byte)EPacketProtoID._00_HEART_BEAT << 4);
            trace(client._endpointIn + ":         >>>>>>> SENDING: " + packetStreamTemp.byteLength);
        }
    }
}
