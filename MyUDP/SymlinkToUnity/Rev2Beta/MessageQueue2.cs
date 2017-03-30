using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyUDP.Rev2Beta {
    public class MessageQueue2 {
        private static bool _isInited = false;
        private static int _DEFAULT_MESSAGES_MAX = 16;

        public static PoolOfBytes POOL_OF_BYTES;

        public int messagesMax = -1;

        private List<Message2> _messages;
        public List<Message2> messages { get { return _messages; } }

        public bool hasMessages { get { return _messages != null && _messages.Count > 0; } }

        private static void InitClass() {
            _isInited = true;
            POOL_OF_BYTES = new PoolOfBytes();
        }

        public MessageQueue2() {
            if (!_isInited) InitClass();

            _messages = new List<Message2>();
            if (messagesMax <= 0) messagesMax = _DEFAULT_MESSAGES_MAX;
        }

        public void AddBytes(byte[] bytes) {
            if (_messages.Count > messagesMax) {
                Client2.traceError("Reached Max Count of Messages; need to process some before adding more bytes to the queue!");
                return;
            }

            byte[] tempBytes = POOL_OF_BYTES.PopBytes();
            bytes.CopyTo(tempBytes, 0);
            _messages.Add(new Message2(Utils.GetTime(), tempBytes));
        }

        public class Message2 {
            public ulong timestamp = 0;
            public byte[] bytes;

            public Message2(ulong timestamp, byte[] bytes) {
                this.timestamp = timestamp;
                this.bytes = bytes;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////

    public class PoolOfBytes :List<byte[]> {
        public static int DEFAULT_COUNT = 512;
        public static int DEFAULT_BYTES_LENGHT = -1;

        public PoolOfBytes() : base(DEFAULT_COUNT) {
            int bytesLength = DEFAULT_BYTES_LENGHT <= 0 ? MyDefaults.DATA_STREAM_SIZE : DEFAULT_BYTES_LENGHT;
            for (int i = 0; i < DEFAULT_COUNT; i++) {
                this.Add( new byte[bytesLength] );
            }
        }

        public byte[] PopBytes() {
            if (this.Count <= 0) return null;
            byte[] bytes = this[0];
            this.RemoveAt(0);

            return bytes;
        }

        public void PushBytes(byte[] bytes) {
            Array.Clear(bytes, 0, bytes.Length);

            this.Add(bytes);
        }
    }
}
