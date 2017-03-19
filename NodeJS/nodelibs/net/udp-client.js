/**
 * Created by Chamberlain on 18/03/2017.
 */

const dgram = require('dgram');
const UDPBuffer = require('./udp-buffer');
const PORT = 11000;

class UDPClient {
	constructor(options) {
		if(!options) options = {};
		var _this = this;
		this.port = options.port || PORT;
		this._isRepeating = false;
		this._repeatID = -1;
		this._udpBuffer = new UDPBuffer();

		var client = this.client = dgram.createSocket('udp4');
		client.on('connect', function(err) {
			if(!options.onConnect) return;
			if(err) return options.onConnect(err);

			options.onConnect && options.onConnect();
		});

		client.on('error', function(err) {
			if(!options.onReceive) return;
			options.onReceive(err);
		});

		client.on('message', function(packet, remote) {
			if(!options.onReceive) return;

			_this._udpBuffer.setPacket(packet);
			options.onReceive(null, _this._udpBuffer, remote, client);
		});
	}

	isRepeating() { return this._isRepeating; }

	///////////////////////////////////////////////////

	send(buffer, cb) {
		this.client.send(buffer, this.port, 'localhost', (err) => {
			if(err) return traceError(err);

			cb && cb();
		});
	}

	close() {
		this.client.close();
		trace("Closing connection....");
	}

	repeatSend(timeMS, cbBuffer) {
		if(!cbBuffer) {
			return traceError("repeatSend() - You must pass a callback function that returns a buffer!");
		}

		if(!timeMS || timeMS<1) timeMS = 250;

		var _this = this;
		this._isRepeating = true;
		this._repeatID = setTimeout(() => {
			var bytesBuffer = cbBuffer(_this._udpBuffer);
			_this.send(bytesBuffer);
			_this.repeatSend(timeMS, cbBuffer);
		}, timeMS);
	}

	repeatStop() {
		if(this._repeatID==-1) return;

		clearTimeout(this._repeatID);
		this._repeatID = -1;
		this._isRepeating = false;
	}
}

module.exports = UDPClient;