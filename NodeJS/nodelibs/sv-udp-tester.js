/**
 * Created by Chamberlain on 18/03/2017.
 */
const dgram = require('dgram');
//const udpServer = dgram.createSocket('udp4');

const STATUSES = { NULL: 0, MESSAGE: 1, LOGIN: 2, LOGOUT: 3 };
const PORT = 11000;

module.exports = function(BIGP) {
	var UDP = BIGP.udp = new UDPClient(PORT);
	UDP.repeatSend(() => {
		return createMessage("Pierre", "Hello! Counting... " + new Date().getTime());
	});

	setTimeout(() => {
		UDP.repeatStop();
		UDP.close();
	}, 2000);
};

//////////////////////////////////////////////////////////

class UDPClient {
	constructor(port) {
		this.port = port;
		this._isRepeating = false;
		this._repeatID = -1;
		this.client = dgram.createSocket('udp4');
		this.client.on('error', function(err) {
			trace(err);
		});

		this.client.on('connect', function() {
			trace("CONNNNNNNNNNECTED!");
		});
	}

	send(buffer, cb) {
		this.client.send(buffer, this.port, 'localhost', (err) => {
			if(err) return traceError(err);

			trace("Message sent!");

			cb && cb();
		});
	}

	close() {
		this.client.close();
		trace("Closing connection....");
	}

	repeatSend(cbBuffer, timeMS) {
		if(!cbBuffer) {
			return traceError("repeatSend() - You must pass a callback function that returns a buffer!");
		}

		if(!timeMS || timeMS<1) timeMS = 250;

		var _this = this;
		this._isRepeating = true;
		this._repeatID = setTimeout(() => {
			var buffer = cbBuffer();
			_this.send(buffer);
			_this.repeatSend(cbBuffer, timeMS);
		}, timeMS);
	}

	repeatStop() {
		if(this._repeatID==-1) return;

		clearTimeout(this._repeatID);
		this._repeatID = -1;
		this._isRepeating = false;
	}

	isRepeating() {
		return this._isRepeating;
	}
}


function createMessage(name, msg) {
	return createBuffer(STATUSES.MESSAGE, Buffer.byteLength(name), Buffer.byteLength(msg), name, msg);
}

function createBuffer() {
	var len = 0;
	var operations = [];
	for(var a=0; a<arguments.length; a++) {
		var value = arguments[a];
		switch(typeof(value)) {
			case 'number': operations.push({method: 'writeInt32LE', args: [value, len]});
				len += 4;
				break;
			case 'string': operations.push({method: 'write', args: [value, len, -1, 'utf8']});
				len += Buffer.byteLength(value);
				break;
		}
	}

	var buf = new Buffer(len);

	operations.forEach(op => {
		buf[op.method].apply(buf, op.args);
	});

	return buf;
}

//client.on('message', function(message, remote) {
//	var addr = remote.address;
//
//	var whole = [];
//	var len = 0;
//	whole.push( message.readInt32LE(len) );
//	var nameLen = message.readInt32LE(len+=4);
//	var msgLen = message.readInt32LE(len+=4);
//	whole.push( message.toString('utf8', len+=4, len += nameLen) );
//	var msg = message.toString('utf8', len, len += msgLen);
//	whole.push( msg );
//
//	tracker++;
//
//	var num = parseInt(msg.match(/\d+/)[0]);
//	if(tracker!=num) {
//		errors.push("Missmatch packet: " + tracker);
//	}
//
//	trace(remote);
//	trace(addr + " : " + whole.join(' ') + " #" + num);
//
//	traceError(errors);
//});

//var keepSending = true;
//var counter = 0;
//
//function sendRepeat() {
//	if(!keepSending) return;
//
//	//traceClear();
//	const buf = createMessage("Pierre", "Hello! Counting... " + counter++);
//
//	send(buf, () => {
//		setTimeout(() => {
//			sendRepeat();
//		}, 1000);
//	});
//}

//sendRepeat();