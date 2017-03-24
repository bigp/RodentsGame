using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyUDP.UnityPreset {
    using Clock;
    using UnityClients = Dictionary<MyUDPServerClient, UnityServerClient>;

    [Flags]
    public enum EClientStatus {
        NEW = 1,
        CONNECTED = 2,
        SLEEPING = 4,
        WAKING = 8,
        DISCONNECTED = 16,
    }

    class UnityServer : MyUDPServer {
        public static float DEFAULT_UPDATE_RATE = 10f;
        private Object thisLock = new Object();

        public Clockwork masterClock;
        public UnityClients clientsUnity;
        public List<UnityServerClient> clientsToForget;

        public float timeForSleep = 8;
        public float timeForForget = 15;
        public DateTime timeStarted;

        public Action<UnityServerClient> OnClientSleeping;
        public Action<UnityServerClient> OnClientWaking;
        public Action<UnityServerClient> OnClientReturned;
        public Action<UnityServerClient> OnClientForgetting;
        
        public double timeSinceStart {
            get {
                DateTime timeNow = DateTime.Now;
                TimeSpan timeDiff = timeNow - timeStarted;
                return timeDiff.TotalMilliseconds;
            }
        }
        public override void Init() {
            clientsUnity = new UnityClients();
            clientsToForget = new List<UnityServerClient>();
            SetPacketType<UnityPacket>();

            timeStarted = DateTime.Now;

            //Hook-up the events:
            this.OnNewClient += OnNewUnityClient;
            this.OnDataReceived += OnUnityDataReceived;

            //Set the "ticker" for sending messages back at given intervals:
            masterClock = new Clockwork().StartAutoUpdate(DEFAULT_UPDATE_RATE);
            masterClock.AddInterleaving(OnReadyToSendPositions, OnReadyToSendRotations);
            masterClock.AddGear("Unity Housekeeping").AddListener(OnUnityCheckClientsAlive);
        }

        public void OnReadyToSendPositions(Gear gear) {
            
        }

        public void OnReadyToSendRotations(Gear gear) {

        }

        private UnityServerClient Resolve(MyUDPServerClient client) {
            if(!clientsUnity.ContainsKey(client)) {
                throw new Exception("Resolve ERROR - Missing 'UnityServerClient' object for client: " + client.ToString());
            }

            return clientsUnity[client];
        }

        private void OnNewUnityClient(MyUDPServerClient client) {
            if(clientsUnity.ContainsKey(client)) {
                throw new Exception("OnNewUnityClient ERROR - Already has the client, NOT NEW: " + client.ToString());
            }

            clientsUnity[client] = new UnityServerClient(client);
        }

        

        private void OnUnityDataReceived(MyUDPPacket packet, MyUDPServerClient client) {
            UnityServerClient unityClient = ManageClientConnection(client);
            packet.CopyTo( unityClient.packetClone );
        }

        private UnityServerClient ManageClientConnection(MyUDPServerClient client) {
            UnityServerClient unityClient = Resolve(client);
            UnityPacket unityPacket = (UnityPacket)packet;

            if (unityClient.HasFlag(EClientStatus.SLEEPING)) {
                unityClient.status &= ~EClientStatus.SLEEPING;
                unityClient.status |= EClientStatus.WAKING;

                if (OnClientWaking != null)
                    OnClientWaking(unityClient);

            } else if (unityClient.HasFlag(EClientStatus.WAKING)) {
                unityClient.status &= ~EClientStatus.WAKING;

                if (OnClientReturned != null)
                    OnClientReturned(unityClient);

            }

            //Alright, set the client's status and time-received stamp:
            double timeNow = timeSinceStart;
            unityClient.timeDiff = timeNow - unityClient.timeLastReceived;
            unityClient.timeLastReceived = timeNow;
            unityClient.status |= EClientStatus.CONNECTED;

            return unityClient;
        }

        private void OnUnityCheckClientsAlive(Gear obj) {
            
            double timeNow = timeSinceStart;

            trace("Checking Alive..." + clientsUnity.Count);
            Log.BufferClear();

            //List<UnityServerClient> clients = clientsUnity.Values;

            foreach (UnityServerClient unityClient in clientsUnity.Values) {
                double diffNow = timeNow - unityClient.timeLastReceived;
                double diffSeconds = diffNow * 0.001f;
                double diffFromForget = timeForForget - diffSeconds;
                int diffInt = (int)(diffFromForget * 2);
                Log.BufferAdd(unityClient.ToString() + ": " + "#".Times(diffInt));

                if(!unityClient.HasFlag(EClientStatus.SLEEPING)) {
                    if (diffSeconds > timeForSleep) {
                        unityClient.status |= EClientStatus.SLEEPING;
                    }
                } else if(!unityClient.HasFlag(EClientStatus.DISCONNECTED)) {
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
                foreach (UnityServerClient unityClient in clientsToForget) {
                    clientsUnity.Remove(unityClient.client);
                    ForgetClient(unityClient.client.endpointIn);
                }

                clientsToForget.Clear();
            }
        }
    }

    class UnityServerClient {
        private static int CLIENT_ID = 1;
        public EClientStatus status = EClientStatus.NEW;
        public MyUDPServerClient client;
        public uint lastACK = 0;
        public double timeLastReceived = 0;
        public double timeDiff = 0;
        public int clientID;
        public UnityPacket packetClone;
        
        public UnityServerClient(MyUDPServerClient client) {
            this.client = client;
            clientID = CLIENT_ID++;
            packetClone = new UnityPacket();
        }

        public bool HasFlag(EClientStatus flag) {
            return (status & flag) != 0;
        }

        public override string ToString() {
            return client.endpointIn.ToString();
        }

        public UnityPacket packet { get { return (UnityPacket)client.packet; } }
    }
}
