using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public class Command {
			public int ackID = -1;
			public int timeOffset = -1;
			public EPacketTypes types;
			public XYZData xyzData = new XYZData();
		}

		public class XYZData {
			public int ackFromServer = -1;
			public double[] position = new double[3];
			public double[] rotation = new double[4];
			public int action;
			public string jsonData;
		}
	}

	public class UnityPacket : MyUDPPacket {
		public ulong clientTime = 0;
		public int numOfCommands = 0;
		public List<Command> commands;

		public string clientTimeFormatted {
			get { return Utils.GetDate(clientTime).ToShortTimeString(); }
		}

		public UnityPacket() : base() {
			commands = new List<Command>();
		}

		public override void EncodeCustom() {
			numOfCommands = commands.Count;

			WriteULongs(clientTime);
			WriteInts(numOfCommands);

			foreach (Command cmd in commands) {
				WriteInts(cmd.ackID);
				WriteInts(cmd.timeOffset);
				WriteBytes((byte)cmd.types);

				if (cmd.types == 0) continue;

				XYZData xyzData = cmd.xyzData;

				//string preview = "";
				Utils.ForEachFlags(cmd.types, (flag) => {

					switch (flag) {
						case EPacketTypes.ACK:
							//preview += "\n  ACK: " + xyzData.ackFromServer;
							WriteInts(xyzData.ackFromServer);
							break;
						case EPacketTypes.ACTION:
							//preview += "\n  ACTION: " + xyzData.action;
							WriteInts(xyzData.action);
							break;
						case EPacketTypes.POSITION:
							//preview += "\n  POS: " + xyzData.position.Join(", ");
							WriteDoubles(xyzData.position);
							break;
						case EPacketTypes.ROTATION:
							//preview += "\n  ROT: " + xyzData.rotation.Join(", ");
							WriteDoubles(xyzData.rotation);
							break;
						case EPacketTypes.JSON:
							//preview += "\n  JSON: " + xyzData.jsonData;
							WriteStrings(xyzData.jsonData);
							break;
						default:
							Log.traceError("Unhandled WRITE Packet Type found!" + flag);
							break;
					}
				});

				//Log.trace("Encoding: " + preview);
			}
		}

		public override void DecodeCustom() {
			clientTime = ReadULong();
			numOfCommands = ReadInt();

			commands.Clear();

			for (int c = 0; c < numOfCommands; c++) {
				Command cmd = new Command();

				cmd.ackID = ReadInt();
				cmd.timeOffset = ReadInt();
				cmd.types = (EPacketTypes)ReadByte();
				
				XYZData xyzData = cmd.xyzData;

				if (cmd.types > 0) {
					Utils.ForEachFlags(cmd.types, (flag) => {
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
								xyzData.jsonData = ReadString();
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
		}
	}
}
