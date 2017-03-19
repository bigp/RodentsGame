/**
 * Created by Chamberlain on 18/03/2017.
 */
class UDPBuffer {
	constructor() {}

	/**
	 * @param packet Buffer
	 */
	setPacket(packet) {
		this._packet = packet;
		this._offset = 0;
	}

	readULong() {
		var big = this._packet.readUInt32LE(this._offset); this._offset+=4;
		var low = this._packet.readUInt32LE(this._offset); this._offset+=4;

		return parseInt(big.toString(16) + low.toString(16), 16);
		//trace(redHex(val));
		//trace(redHex(low));
		//trace(redHex(val));
		//return val;
	}

	readInt() {
		var val = this._packet.readInt32LE(this._offset);
		this._offset += 4;
		return val;
	}

	readByte() {
		var val = this._packet[this._offset];
		this._offset++;
		return val;
	}

	readString(size=-1) {
		if(size<0) size = this.readInt();

		return this._packet.toString('utf8', this._offset, this._offset += size);;
	}

	createBuffer(args) {
		if(!args || !args.length) {
			traceError("No arguments passed to transform into a buffer!");
			return Buffer.from("null");
		}

		var len = 0;
		var operations = [];

		for(var a=0; a<args.length; a++) {
			var value = args[a];
			var type = typeof(value);

			if(_.isArray(value)) {
				type = 'byte';
				value = value[0];
			} else if(type=='object') {
				type = value.type;
				value = value.value;
			}

			switch(type) {
				case '1':
				case 'uint8':
				case 'char':
				case 'byte':
					operations.push({method: 'writeUInt8', args: [value, len]});
					len+=1;
					break;
				case '8':
				case 'ulong':
				case 'uint64':
				case 'date':
					const MAX_UINT32 = 0xFFFFFFFF;
					const big = ~~(value / MAX_UINT32);
					const low = ((value & 0xFFFFFFFF) >>> 0);

					//trace(redHex(value));
					//trace("big: " + redHex(big));
					//trace("low: " + redHex(low));

					operations.push({method: 'writeUInt32LE', args: [big, len]});
					len+=4;
					operations.push({method: 'writeUInt32LE', args: [low, len]});
					len+=4;
					break;
				case '4':
				case 'int':
				case 'int32':
				case 'number':
					operations.push({method: 'writeInt32LE', args: [value, len]});
					len += 4;
					break;
				case 'word':
				case 'string':
					var strBytes = Buffer.byteLength(value);
					operations.push({method: 'writeInt32LE', args: [strBytes, len]});
					len += 4;
					operations.push({method: 'write', args: [value, len, len+=strBytes, 'utf8']});
					break;
			}
		}

		var buf = new Buffer(len);
		buf.write
		operations.forEach(op => {
			buf[op.method].apply(buf, op.args);
		});

		return buf;
	}
}

module.exports = UDPBuffer;