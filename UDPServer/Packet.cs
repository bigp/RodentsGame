using System;
using System.Collections.Generic;
using System.Text;

namespace MyUDP {
	// Packet Structure
	// =======================================================================================
	// Description   -> |dataIdentifier|name length|message length|    name   |    message   |
	// Size in bytes -> |       4      |     4     |       4      |name length|message length|
	public enum EnumPacketPart { Null, Message, LogIn, LogOut }

	public class MyUDPPacket {
		private EnumPacketPart _identifier;
		private string _name;
		private string _message;

		private int _byteIndex = 0;
		private byte[] _byteStream;
		private List<byte> _bytesList;

		public static Encoding UTF8 { get { return Encoding.UTF8; } }
		public string Name { get { return _name; } set { _name = value; } }
		public string Message { get { return _message; } set { _message = value; } }
		public EnumPacketPart Identifier { get { return _identifier; } set { _identifier = value; } }

		/////////////////////////////////////////////////////////

		public MyUDPPacket() {
			this._identifier = EnumPacketPart.Null;
			this._message = null;
			this._name = null;

			_bytesList = new List<byte>();
		}

		public void DecodePacket(byte[] byteStream) {
			ResetByteIndex(byteStream);

			this._identifier = (EnumPacketPart)ReadInt(); // Read the data identifier from the beginning of the stream (4 bytes)

			int nameLength = ReadInt(); // Read the length of the NAME (4 bytes)
			int msgLength = ReadInt(); // Read the length of the MESSAGE (4 bytes)

			this._name = ReadString(nameLength); // Read the NAME field
			this._message = ReadString(msgLength); // Read the MESSAGE field
		}

		///////////////////////////////////////////////////////// UTILITY METHODS (Read / Write bytes, tracks index)

		private void ResetByteIndex(byte[] dataStream) {
			_byteIndex = 0;
			_byteStream = dataStream;
		}

		private int ReadInt() {
			int value = BitConverter.ToInt32(_byteStream, _byteIndex);
			_byteIndex += 4;
			return value;
		}

		//private string[] ReadStrings(int numStrings) {

		//}

		private string ReadString(int numChars) {
			if (numChars <= 0) return null;
			string value = UTF8.GetString(_byteStream, _byteIndex, numChars);
			_byteIndex += numChars;
			return value;
		}

		private void WriteInt(params int[] values) {
			foreach (int value in values) {
				_bytesList.AddRange(BitConverter.GetBytes(value));
			}
		}

		private void WriteStringLengths(params string[] values) {
			foreach (string value in values) {
				WriteInt(value == null ? 0 : value.Length);
			}
		}

		private void WriteStrings(params string[] values) {
			foreach (string value in values) {
				if (value == null) continue;
				_bytesList.AddRange(UTF8.GetBytes(value));
			}
		}

		/////////////////////////////////////////////////////////

		// Converts the packet into a byte array for sending/receiving 
		public byte[] GetDataStream() {
			_bytesList.Clear();

			WriteInt((int)this._identifier);             // Add the dataIdentifier
			WriteStringLengths(this._name, this._message);    // Add the length of name, message.
			WriteStrings(this._name, this._message);          // Add the name, message

			return _bytesList.ToArray();
		}
	}
}