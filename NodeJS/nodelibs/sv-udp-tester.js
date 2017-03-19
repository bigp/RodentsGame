/**
 * Created by Chamberlain on 18/03/2017.
 */

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
//  - TIME OFFSET	| int(4)       <== Delta of time from the CLIENT_TIME for each commands.
//  - TYPES         | byte(1)      <== Combination of type flags
//  - XYZ-DATA...   | type-lengths <== Could be combination of uint, byte, long, etc.
//  - JSON-length   | int(4)       <== if TYPE includes
//  - JSON-DATA...  | JSON-length
// ===================================

const UDPClient = require('./net/udp-client');
const EPacketTypes = { ACK: 1, POSITION: 2, ROTATION: 4, ACTION: 8, JSON: 64 };
const EPacketSizes = {};
EPacketSizes[EPacketTypes.ACK] = 4;          //32-bit * 1: Int32 to 'trim' current acknowledged commands.
EPacketSizes[EPacketTypes.POSITION] = 4 * 3; //32-bit * 3: Vector3(X, Y, Z)
EPacketSizes[EPacketTypes.ROTATION] = 4 * 4; //32-bit * 4: Quaternion(X, Y, Z, W)
EPacketSizes[EPacketTypes.ACTION] = 1;		 //1 byte (256 possible choices)
EPacketSizes[EPacketTypes.JSON] = -1;

global.redHex = function (value) {
	return value.toString(16).red;
};

module.exports = function(BIGP) {
	var sampleJSON = JSON.stringify({x: 32}); // , y: 2, z: 3, isVisible: true

	var commandsTest = [
		{ack: 0, frame: 1, types: EPacketTypes.JSON, jsonData: sampleJSON},
		{ack: 1, frame: 2, types: EPacketTypes.JSON, jsonData: sampleJSON}
	];

	var UDP = BIGP.udp = new UDPClient({
		onReceive(err, udpBuffer, remote, client) {
			if(err) return traceError(err);

			var clientTime = udpBuffer.readULong();
			var numCommands = udpBuffer.readInt();
			var clientLocalTime = new Date(clientTime).toLocaleTimeString();

			var cmdArr = [];

			for(var c=0; c<numCommands; c++) {
				var ack = udpBuffer.readInt();
				var frameID = udpBuffer.readInt();
				var types = udpBuffer.readByte();

				//TYPES..........

				var jsonData = udpBuffer.readString();

				cmdArr.push(`\n - ACK ${ack} frame ${frameID} types ${types}, "${jsonData}"` );
			}

			var cmdStr = cmdArr.join('');


			trace(`${clientLocalTime} (${numCommands})${cmdStr}\n`.cyan);
		}
	});

	UDP.repeatSend(500, (updBuffer) => {
		var now = new Date().getTime();
		var data = [ {type:'date', value: now}, commandsTest.length ];

		commandsTest.forEach(cmd => {
			//For each commands, add the ACK, FRAME_ID, TYPES and JSON-DATA...
			data.push( cmd.ack, cmd.frame, [cmd.types], /* TYPES...*/ cmd.jsonData );
		});

		return updBuffer.createBuffer(data);
	});

	//Close the connection after some time.
	setTimeout(() => {
		UDP.repeatStop();
		UDP.close();
	}, 4000);
};



