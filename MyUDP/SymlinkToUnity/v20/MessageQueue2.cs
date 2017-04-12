using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyUDP.v20 {
    public class MessageQueue2 {
        private static bool _isInited = false;
        private static int _DEFAULT_MESSAGES_MAX = 4;

        public static PoolOfMessages POOL_OF_MESSAGES;

        public int messagesMax = -1;

        private List<Message2> _messages;
        public List<Message2> messages { get { return _messages; } }

        public bool hasMessages { get { return _messages != null && _messages.Count > 0; } }

        private static void InitClass() {
            _isInited = true;
            POOL_OF_MESSAGES = new PoolOfMessages();
        }

        public MessageQueue2() {
            if (!_isInited) InitClass();

            _messages = new List<Message2>();
            if (messagesMax <= 0) messagesMax = _DEFAULT_MESSAGES_MAX;
        }

        public void AddBytes(byte[] bytes) {
            if (_messages.Count >= messagesMax) {
                Client2.traceError("Reached Max Count of Messages; need to process some before adding more bytes to the queue!");
                return;
            }

            Message2 tempMsg = POOL_OF_MESSAGES.Pop();
            bytes.CopyTo(tempMsg.bytes, 0);
            tempMsg.timestamp = Utils.GetTime();
            _messages.Add(tempMsg);
        }

        internal void RecycleMessages() {
            foreach(Message2 msg in _messages) {
                POOL_OF_MESSAGES.Push(msg);
            }

            _messages.Clear();
        }
    }

    public class Message2 {
        public ulong timestamp = 0;
        public byte[] bytes;

        public Message2(ulong timestamp, byte[] bytes) {
            this.timestamp = timestamp;
            this.bytes = bytes;
        }

        public Message2 Recycle() {
            MessageQueue2.POOL_OF_MESSAGES.Push( this );
            return this;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////

    public class PoolOfMessages :List<Message2> {
        public static int DEFAULT_COUNT = 512;
        public static int DEFAULT_BYTES_LENGHT = -1;

        public PoolOfMessages() : base(DEFAULT_COUNT) {
            int bytesLength = DEFAULT_BYTES_LENGHT <= 0 ? MyDefaults.DATA_STREAM_SIZE : DEFAULT_BYTES_LENGHT;
            for (int i = 0; i < DEFAULT_COUNT; i++) {
                this.Add( new Message2(0, new byte[bytesLength]) );
            }
        }

        public Message2 Pop() {
            if (this.Count <= 0) return null;
            Message2 msg = this[0];
            this.RemoveAt(0);

            return msg;
        }

        public void Push(Message2 msg) {
            Array.Clear(msg.bytes, 0, msg.bytes.Length);

            this.Add(msg);
        }
    }
}
