using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyUDP.v20 {
    public class PacketStream2 {
        public static Encoding UTF8 { get { return Encoding.UTF8; } }

        private int _byteLength = 0;
        private byte[] _byteStream;
        //private List<byte> _bytesList;

        public PacketStream2(int size = -1) {
            //_bytesList = new List<byte>();
            _byteStream = new byte[size > 0 ? size : MyDefaults.DATA_STREAM_SIZE];
        }

        public byte[] byteStream { get { return _byteStream; } }
        public int byteLength { get { return _byteLength; } }

        public void ResetByteIndex() {
            _byteLength = 0;
            //_bytesList.Clear();
        }

        ///////////////////////////////////////////////////////// UTILITY METHODS (Read / Write bytes, tracks index)

        public byte ReadByte() {
            byte value = _byteStream[_byteLength];
            _byteLength += 1;
            return value;
        }

        public short ReadShort() {
            short value = BitConverter.ToInt16(_byteStream, _byteLength);
            _byteLength += 2;
            return value;
        }

        public int ReadInt() {
            int value = BitConverter.ToInt32(_byteStream, _byteLength);
            _byteLength += 4;
            return value;
        }

        public uint ReadUInt() {
            uint value = BitConverter.ToUInt32(_byteStream, _byteLength);
            _byteLength += 4;
            return value;
        }

        public ulong ReadULong() {
            ulong value = BitConverter.ToUInt64(_byteStream, _byteLength);
            _byteLength += 8;
            return value;
        }

        public double ReadDouble() {
            double value = BitConverter.ToDouble(_byteStream, _byteLength);
            _byteLength += 8;
            return value;
        }

        public string ReadString(int numChars = -1) {
            if (numChars < 0) numChars = ReadInt();
            if (numChars == 0) return null;
            string value = UTF8.GetString(_byteStream, _byteLength, numChars);
            _byteLength += numChars;
            return value;
        }

        /////////////////////////////////////////////////////////

        public void WriteDoubles(params double[] values) {
            foreach (double value in values) {
                BitConverter.GetBytes(value).CopyTo(_byteStream, _byteLength);
                //_bytesList.AddRange(BitConverter.GetBytes(value));
                _byteLength += 8;
            }
        }

        //internal void CopyTo(MyUDPPacket dest) {
        //    dest.Decode(_byteStream);
        //}

        public void WriteBytes(params int[] ints) {
            byte[] bytes = Array.ConvertAll<int, byte>(ints, Convert.ToByte);
            WriteBytes(bytes);
        }

        public void WriteBytes(params byte[] bytes) {
            //_bytesList.AddRange(bytes);
            bytes.CopyTo(_byteStream, _byteLength);
            _byteLength += bytes.Length;
        }

        public void WriteInts(params int[] values) {
            foreach (int value in values) {
                //_bytesList.AddRange();
                BitConverter.GetBytes(value).CopyTo(_byteStream, _byteLength);
                _byteLength += 4;
            }
        }

        public void WriteUInts(params uint[] values) {
            foreach (uint value in values) {
                BitConverter.GetBytes(value).CopyTo(_byteStream, _byteLength);
                //_bytesList.AddRange(BitConverter.GetBytes(value));
                _byteLength += 4;
            }
        }

        public void WriteLong(params long[] values) {
            foreach (long value in values) {
                BitConverter.GetBytes(value).CopyTo(_byteStream, _byteLength);
                //_bytesList.AddRange(BitConverter.GetBytes(value));
                _byteLength += 8;
            }
        }

        public void WriteULongs(params ulong[] values) {
            foreach (ulong value in values) {
                BitConverter.GetBytes(value).CopyTo(_byteStream, _byteLength);
                //_bytesList.AddRange(BitConverter.GetBytes(value));
                _byteLength += 8;
            }
        }

        public void WriteShorts(params short[] values) {
            foreach (short value in values) {
                BitConverter.GetBytes(value).CopyTo(_byteStream, _byteLength);
                //_bytesList.AddRange(BitConverter.GetBytes(value));
                _byteLength += 2;
            }
        }

        public void WriteStrings(params string[] values) {
            foreach (string value in values) {
                WriteInts(string.IsNullOrEmpty(value) ? 0 : value.Length);
                if (value == null) continue;
                byte[] strBytes = UTF8.GetBytes(value);
                strBytes.CopyTo(_byteStream, _byteLength);
                //_bytesList.AddRange(strBytes);
                _byteLength += strBytes.Length;
            }
        }
    }
}
