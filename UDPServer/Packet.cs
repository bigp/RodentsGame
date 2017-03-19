using System;
using System.Collections.Generic;
using System.Text;

namespace MyUDP {
	using Packet;

	// ===================================
	//       -- PACKET STRUCTURE --
	// 
	// Description        Size (bytes)
	// ===================================
	// CLIENT_TIME      | uint(4)
	// NUM_OF_COMMANDS  | int(4)
	//
	// [commands]       | ...
	//  - ACK ID        | int(4)       <== Acknoledge ID, increments during the cycle of a client connection.
	//  - FRAME ID      | int(4)       <== Delta of time from the CLIENT_TIME for each commands.
	//  - TYPES         | byte(1)      <== Combination of type flags
	//  - XYZ-DATA...   | type-lengths <== Could be combination of uint, byte, long, etc.
	//  - JSON-length   | int(4)       <== if TYPE includes
	//  - JSON-DATA...  | JSON-length
	// ===================================
	namespace Packet {
		[Flags]
		public enum EPacketTypes {
			JSON = 64,
		}

		public struct Command {
			public int ackID;
			public int timeOffset;
			public EPacketTypes types;
			public byte[] typeData;
			public string jsonData;
		}
	}

	public class MyPacket {
		private int _byteIndex = 0;
		private byte[] _byteStream;
		private List<byte> _bytesList;

		public static Encoding UTF8 { get { return Encoding.UTF8; } }
		
		public ulong clientTime = 0;
		public int numOfCommands = 0;
		public List<Command> commands;

		/////////////////////////////////////////////////////////

		public MyPacket() {
			_bytesList = new List<byte>();
			commands = new List<Command>();
		}

		public void DecodePacket(byte[] byteStream) {
			ResetByteIndex(byteStream);

			clientTime = ReadULong();
			numOfCommands = ReadInt();

			commands.Clear();

			for(int c=0; c<numOfCommands; c++) {
				Command cmd = new Command();

				cmd.ackID = ReadInt();
				cmd.timeOffset = ReadInt();
				cmd.types = (EPacketTypes)ReadByte();
				cmd.jsonData = ReadString();

				Log.trace("{0}, {1}, {2}, {3}", cmd.ackID, cmd.timeOffset, (int)cmd.types, cmd.jsonData);

				//Log.trace("JSONData -> " + cmd.jsonData);

				commands.Add(cmd);
			}
		}

		public byte[] EncodePacket() {
			_bytesList.Clear();

			WriteULongs(clientTime);
			WriteInts(numOfCommands);

			foreach(Command cmd in commands) {
				WriteInts(cmd.ackID);
				WriteInts(cmd.timeOffset);
				WriteBytes((byte) cmd.types);
				WriteStrings(cmd.jsonData);
			}

			return _bytesList.ToArray();
		}

		///////////////////////////////////////////////////////// UTILITY METHODS (Read / Write bytes, tracks index)

		private void ResetByteIndex(byte[] dataStream) {
			_byteIndex = 0;
			_byteStream = dataStream;
		}

		private byte ReadByte() {
			byte value = _byteStream[_byteIndex];
			_byteIndex += 1;
			return value;
		}

		private short ReadShort() {
			short value = BitConverter.ToInt16(_byteStream, _byteIndex);
			_byteIndex += 2;
			return value;
		}

		private int ReadInt() {
			int value = BitConverter.ToInt32(_byteStream, _byteIndex);
			_byteIndex += 4;
			return value;
		}

		private uint ReadUInt() {
			uint value = BitConverter.ToUInt32(_byteStream, _byteIndex);
			_byteIndex += 4;
			return value;
		}

		private ulong ReadULong() {
			ulong value = BitConverter.ToUInt64(_byteStream, _byteIndex);
			_byteIndex += 8;
			return value;
		}

		private double ReadDouble() {
			double value = BitConverter.ToDouble(_byteStream, _byteIndex);
			_byteIndex += 8;
			return value;
		}

		private string ReadString(int numChars=-1) {
			if(numChars==0) return null;
			if (numChars < 0) numChars = ReadInt();
			string value = UTF8.GetString(_byteStream, _byteIndex, numChars);
			_byteIndex += numChars;
			return value;
		}

		/////////////////////////////////////////////////////////

		private void WriteDoubles(params double[] values) {
			foreach (double value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
			}
		}

		private void WriteBytes(params byte[] bytes) {
			_bytesList.AddRange(bytes);
		}

		private void WriteInts(params int[] values) {
			foreach (int value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
			}
		}

		private void WriteUInts(params uint[] values) {
			foreach (uint value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
			}
		}

		private void WriteLong(params long[] values) {
			foreach (long value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
			}
		}

		private void WriteULongs(params ulong[] values) {
			foreach (ulong value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
			}
		}

		private void WriteShorts(params short[] values) {
			foreach (short value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
			}
		}

		private void WriteStrings(params string[] values) {
			foreach (string value in values) {
				WriteInts(string.IsNullOrEmpty(value) ? 0 : value.Length);
				if (value == null) continue;
				_bytesList.AddRange(UTF8.GetBytes(value));
			}
		}
	}
}