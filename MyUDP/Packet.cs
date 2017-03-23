using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyUDP {
	using Core;

	public abstract class MyUDPPacket {
		private int _byteLength = 0;
		private byte[] _byteStream;
		private List<byte> _bytesList;

		public static Encoding UTF8 { get { return Encoding.UTF8; } }
		
		public int byteLength { get { return _byteLength; } }

		///////////////////////////////////////////////////////// Methods to Implement in Subclasses:

		public abstract void EncodeCustom();
		public abstract void DecodeCustom();

		/////////////////////////////////////////////////////////

		public MyUDPPacket() {
			_bytesList = new List<byte>();
			_byteStream = new byte[MyDefaults.DATA_STREAM_SIZE];
		}

		private void ResetByteIndex() {
			_byteLength = 0;
			_bytesList.Clear();
		}

		public void Decode(byte[] source) {
			source.CopyTo(_byteStream, 0);

			ResetByteIndex();

			DecodeCustom();
		}

		public byte[] Encode(byte[] destination = null) {
			ResetByteIndex();

			EncodeCustom();

			if (destination != null) {
				_bytesList.CopyTo(destination);
				return destination;
			}

			_bytesList.CopyTo(_byteStream);

			return _byteStream;
		}

		///////////////////////////////////////////////////////// UTILITY METHODS (Read / Write bytes, tracks index)

		protected byte ReadByte() {
			byte value = _byteStream[_byteLength];
			_byteLength += 1;
			return value;
		}

		protected short ReadShort() {
			short value = BitConverter.ToInt16(_byteStream, _byteLength);
			_byteLength += 2;
			return value;
		}

		protected int ReadInt() {
			int value = BitConverter.ToInt32(_byteStream, _byteLength);
			_byteLength += 4;
			return value;
		}

		protected uint ReadUInt() {
			uint value = BitConverter.ToUInt32(_byteStream, _byteLength);
			_byteLength += 4;
			return value;
		}

		protected ulong ReadULong() {
			ulong value = BitConverter.ToUInt64(_byteStream, _byteLength);
			_byteLength += 8;
			return value;
		}

		protected double ReadDouble() {
			double value = BitConverter.ToDouble(_byteStream, _byteLength);
			_byteLength += 8;
			return value;
		}

		protected string ReadString(int numChars=-1) {
			if (numChars < 0) numChars = ReadInt();
			if (numChars == 0) return null;
			string value = UTF8.GetString(_byteStream, _byteLength, numChars);
			_byteLength += numChars;
			return value;
		}

		/////////////////////////////////////////////////////////

		protected void WriteDoubles(params double[] values) {
			foreach (double value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength+=8;
			}
		}

		protected void WriteBytes(params byte[] bytes) {
			_bytesList.AddRange(bytes);
			_byteLength += bytes.Length;
		}

		protected void WriteInts(params int[] values) {
			foreach (int value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 4;
			}
		}

		protected void WriteUInts(params uint[] values) {
			foreach (uint value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 4;
			}
		}

		protected void WriteLong(params long[] values) {
			foreach (long value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 8;
			}
		}

		protected void WriteULongs(params ulong[] values) {
			foreach (ulong value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 8;
			}
		}

		protected void WriteShorts(params short[] values) {
			foreach (short value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 2;
			}
		}

		protected void WriteStrings(params string[] values) {
			foreach (string value in values) {
				WriteInts(string.IsNullOrEmpty(value) ? 0 : value.Length);
				if (value == null) continue;
				byte[] strBytes = UTF8.GetBytes(value);
				_bytesList.AddRange(strBytes);
				_byteLength += strBytes.Length;
			}
		}
	}
}