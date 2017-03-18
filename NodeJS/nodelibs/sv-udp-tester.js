/**
 * Created by Chamberlain on 18/03/2017.
 */
const dgram = require('dgram');
//const udpServer = dgram.createSocket('udp4');
const client = dgram.createSocket('udp4');
const STATUSES = { NULL: 0, MESSAGE: 1, LOGIN: 2, LOGOUT: 3 };
const PORT = 11000;

module.exports = function(BIGP) {

	function send(data, cb) {
		client.send(data, PORT, 'localhost', (err) => {
			if(err) return traceError(err);

			trace("Message sent!");

			cb && cb();
		});
	}

	function close() {
		client.close();
		trace("Closing connection....");
	}

	trace("UDP CLIENT TEST!");

	client.on('error', function(err) {
		trace(err);
	});

	client.on('connect', function() {
		trace("CONNNNNNNNNNECTED!");
	});

	var tracker = -1;
	var errors = [];
	client.on('message', function(message, remote) {
		var addr = remote.address;

		var whole = [];
		var len = 0;
		whole.push( message.readInt32LE(len) );
		var nameLen = message.readInt32LE(len+=4);
		var msgLen = message.readInt32LE(len+=4);
		whole.push( message.toString('utf8', len+=4, len += nameLen) );
		var msg = message.toString('utf8', len, len += msgLen);
		whole.push( msg );

		tracker++;

		var num = parseInt(msg.match(/\d+/)[0]);
		if(tracker!=num) {
			errors.push("Missmatch packet: " + tracker);
		}

		trace(remote);
		trace(addr + " : " + whole.join(' ') + " #" + num);

		traceError(errors);
	});

	var keepSending = true;
	var counter = 0;

	function sendRepeat() {
		if(!keepSending) return;

		//traceClear();
		const buf = createMessage("Pierre", "Hello! Counting... " + counter++);

		send(buf, () => {
			setTimeout(() => {
				sendRepeat();
			}, 1000);
		});
	}

	sendRepeat();

	//setTimeout(() => {
	//	keepSending = false;
	//	close();
	//}, 3000);
};

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