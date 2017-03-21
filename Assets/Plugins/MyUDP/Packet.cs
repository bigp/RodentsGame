using System;
using System.Collections;
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
	//  - FRAME ID      | int(4)       <== Delta of time / frame ID since the start of the game from the CLIENT side.
	//  - TYPES         | byte(1)      <== Combination of type flags
	//  - XYZ-DATA...   | type-lengths <== Could be combination of uint, byte, long, etc.
	//  - JSON-length   | int(4)       <== if TYPE includes
	//  - JSON-DATA...  | JSON-length
	// ===================================
	namespace Packet {
		[Flags]
		public enum EPacketTypes {
			ACK = 1,
			ACTION = 2,
			POSITION = 4,
			ROTATION = 8,
			JSON = 64
		}

		public struct Command {
			public int ackID;
			public int timeOffset;
			public EPacketTypes types;
			public XYZData xyzData;
			public string jsonData;
		}

		public struct XYZData {
			public int ackFromServer;
			public double[] position;
			public double[] rotation;
			public int action;
		}
	}

	public class MyPacket {
		private int _byteLength = 0;
		private byte[] _byteStream;
		private List<byte> _bytesList;

		public static Encoding UTF8 { get { return Encoding.UTF8; } }
		
		public ulong clientTime = 0;
		public int numOfCommands = 0;
		public List<Command> commands;

		public int byteLength { get { return _byteLength; } }

		public string clientTimeFormatted {
			get { return Utils.GetDate(clientTime).ToShortTimeString(); }
		}

		/////////////////////////////////////////////////////////

		public MyPacket() {
			_bytesList = new List<byte>();
			commands = new List<Command>();
		}

		private void ResetByteIndex(byte[] dataStream=null) {
			_byteLength = 0;
			_bytesList.Clear();

			if (dataStream==null) return;
			_byteStream = dataStream;
		}

		public void DecodeFrom(byte[] bytes) {
			ResetByteIndex(bytes);
			
			clientTime = ReadULong();
			numOfCommands = ReadInt();

			commands.Clear();

			for(int c=0; c<numOfCommands; c++) {
				Command cmd = new Command();

				cmd.ackID = ReadInt();
				cmd.timeOffset = ReadInt();
				cmd.types = (EPacketTypes)ReadByte();
				cmd.jsonData = null;

				XYZData xyzData = new XYZData();

				if (cmd.types>0) {
					Utils.ForEachFlags(cmd.types, (flag) => {
						Log.trace(flag);

						switch (flag) {
							case EPacketTypes.ACK:
								xyzData.ackFromServer = ReadInt();
								break;
							case EPacketTypes.ACTION:
								xyzData.action = ReadInt();
								break;
							case EPacketTypes.POSITION:
								xyzData.position = new double[3];
								xyzData.position[0] = ReadDouble();
								xyzData.position[1] = ReadDouble();
								xyzData.position[2] = ReadDouble();
								break;
							case EPacketTypes.ROTATION:
								xyzData.rotation = new double[4];
								xyzData.rotation[0] = ReadDouble();
								xyzData.rotation[1] = ReadDouble();
								xyzData.rotation[2] = ReadDouble();
								xyzData.rotation[3] = ReadDouble();
								break;
							case EPacketTypes.JSON:
								cmd.jsonData = ReadString();
								break;
							default:
								Log.traceError("Unhandled READ Packet Type found!" + flag);
								break;
						}
					});
				}

				cmd.xyzData = xyzData;

				commands.Add(cmd);
			}

			bytes.CopyTo(_byteStream, 0);
		}

		public byte[] EncodeTo(byte[] destination=null) {
			ResetByteIndex();

			WriteULongs(clientTime);
			WriteInts(numOfCommands);

			foreach(Command cmd in commands) {
				WriteInts(cmd.ackID);
				WriteInts(cmd.timeOffset);
				WriteBytes((byte) cmd.types);

				if(cmd.types==0) continue;

				XYZData xyzData = cmd.xyzData;

				Log.trace("Encoding: " + cmd.types);

				Utils.ForEachFlags(cmd.types, (flag) => {
					
					switch (flag) {
						case EPacketTypes.ACK:
							WriteInts(xyzData.ackFromServer);
							break;
						case EPacketTypes.ACTION:
							WriteInts(xyzData.action);
							break;
						case EPacketTypes.POSITION:
							WriteDoubles(xyzData.position);
							break;
						case EPacketTypes.ROTATION:
							WriteDoubles(xyzData.rotation);
							break;
						case EPacketTypes.JSON:
							WriteStrings(cmd.jsonData);
							break;
						default:
							Log.traceError("Unhandled WRITE Packet Type found!" + flag);
							break;
					}
				});
			}

			if(destination!=null) {
				_bytesList.CopyTo(destination);
				return destination;
			}

			return _bytesList.ToArray();
		}

		///////////////////////////////////////////////////////// UTILITY METHODS (Read / Write bytes, tracks index)

		private byte ReadByte() {
			byte value = _byteStream[_byteLength];
			_byteLength += 1;
			return value;
		}

		private short ReadShort() {
			short value = BitConverter.ToInt16(_byteStream, _byteLength);
			_byteLength += 2;
			return value;
		}

		private int ReadInt() {
			int value = BitConverter.ToInt32(_byteStream, _byteLength);
			_byteLength += 4;
			return value;
		}

		private uint ReadUInt() {
			uint value = BitConverter.ToUInt32(_byteStream, _byteLength);
			_byteLength += 4;
			return value;
		}

		private ulong ReadULong() {
			ulong value = BitConverter.ToUInt64(_byteStream, _byteLength);
			_byteLength += 8;
			return value;
		}

		private double ReadDouble() {
			double value = BitConverter.ToDouble(_byteStream, _byteLength);
			_byteLength += 8;
			return value;
		}

		private string ReadString(int numChars=-1) {
			if(numChars==0) return null;
			if (numChars < 0) numChars = ReadInt();
			string value = UTF8.GetString(_byteStream, _byteLength, numChars);
			_byteLength += numChars;
			return value;
		}

		/////////////////////////////////////////////////////////

		private void WriteDoubles(params double[] values) {
			foreach (double value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength+=8;
			}
		}

		private void WriteBytes(params byte[] bytes) {
			_bytesList.AddRange(bytes);
			_byteLength += bytes.Length;
		}

		private void WriteInts(params int[] values) {
			foreach (int value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 4;
			}
		}

		private void WriteUInts(params uint[] values) {
			foreach (uint value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 4;
			}
		}

		private void WriteLong(params long[] values) {
			foreach (long value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 8;
			}
		}

		private void WriteULongs(params ulong[] values) {
			foreach (ulong value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 8;
			}
		}

		private void WriteShorts(params short[] values) {
			foreach (short value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
				_byteLength += 2;
			}
		}

		private void WriteStrings(params string[] values) {
			foreach (string value in values) {
				WriteInts(string.IsNullOrEmpty(value) ? 0 : value.Length);
				_byteLength += 4;
				if (value == null) continue;
				byte[] strBytes = UTF8.GetBytes(value);
				_bytesList.AddRange(strBytes);
				_byteLength += strBytes.Length;
			}
		}
	}
}