var GLOBALS;
if(isNode()) GLOBALS = global;
else {
	GLOBALS = window;
	if(!GLOBALS.console) GLOBALS.console = {log: function() {}};
}

function isNode() {
	return typeof module !== 'undefined' && module.exports;
}

var p = Array.prototype;

p.first = function() {
	if(this.length>0) return this[0];
	return null;
};

p.last = function() {
	if(this.length>0) return this[this.length-1];
	return null
};

p.remove = function(item) {
	var id = this.indexOf(item);
	if(id>-1) this.splice(id, 1);
	return id;
};

p.has = function(item) {
	return this.indexOf(item)>-1;
};

Array._map_toKeyValues = function(name,i) {
	return {value: i, name: name};
};

p.toKeyValues = function() {
	return this.map(Array._map_toKeyValues);
};

p.rotate = (function() {
	var unshift = Array.prototype.unshift,
		splice = Array.prototype.splice;

	return function(c) {
		var len = this.length >>> 0,
			count = c >> 0;

		unshift.apply(this, splice.call(this, count % len, len));
		return this;
	};
})();

/////////////////

p = String.prototype;

p.has = function has(str) {
	return this.indexOf(str)>-1;
};

p.rep = function rep(obj) {
	var regex, str = this.toString();
	
	if(_.isArray(obj)) {
		for(var i=obj.length; --i>=0;) {
			regex = new RegExp("\\$"+i, "g");
			str = str.replace(regex, obj[i]);
		}
	} else {
		for(var o in obj) {
			regex = new RegExp("\\$"+o, "g");
			str = str.replace(regex, obj[o]);
		}
	}

	return str;
};

p.fixSlashes = function() {
	var str = this.toString().replace(/\\/g, "/");
	return str.endsWith("/") ? str.substr(0,str.length-1) : str;
};

p.toPath = function() {
	return {
		ext: this.substring(this.lastIndexOf('.')),
		path: this.substring(0, this.lastIndexOf('/')+1),
		filename: this.substring(this.lastIndexOf('/')+1, this.lastIndexOf('.'))
	}
};

p.wrap = function(char) {
	if(!char) char = '"';
	return char + this.toString() + char;
};

"".endsWith || (function() {
	p.endsWith = function(searchString, position) {
		var subjectString = this.toString();
		if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
			position = subjectString.length;
		}
		position -= searchString.length;
		var lastIndex = subjectString.lastIndexOf(searchString, position);
		return lastIndex !== -1 && lastIndex === position;
	};
})();

var regexEmoji = /:([a-z0-9\-\_]*):/gi;

function toEmoji(str) {
	return str.replace(regexEmoji, '<i class="em em-$1"></i>');
}

function toIcon(str) {
	return str.replace(regexEmoji, '<i class="fa fa-$1"></i>')
}

p.camelToTitleCase = function() {
	var text = this.toString();
	var result = text.replace( /([A-Z])/g, " $1" );
	return result.charAt(0).toUpperCase() + result.slice(1);
};

//////////////////////////////////////////////////////////////

Function.prototype.defer = function() {
	var _this = this, args = arguments;
	_.defer(function() { _this.apply(null, args); });
};

//////////////////////////////////////////////////////////////

_.isTruthy = function(bool) {
	return bool===true || "true,1,on,yes".has(bool);
};

_.mapRename = function(obj, filter) {
	var newObj = {};
	_.mapObject(obj, (val, key) => {
		newObj[filter(key)] = val;
	});
	return newObj;
};

_.jsonClone = function(data) {
	return JSON.parse(JSON.stringify(data));
};

_.jsonPretty = function(obj, indent) {
	if(!indent) indent = 2;

	return JSON.stringify(obj, function(k,v) {
		//Check if this is a leaf-object with no child Arrays or Objects:
		for(var p in v) {
			if(_.isArray(v[p]) || _.isObject(v[p])) {
				return v;
			}
		}

		return JSON.stringify(v);

		//Cleanup the escaped strings mess the above generated:
	}, indent).replace(/\\/g, '')
		.replace(/\"\[/g, '[')
		.replace(/\]\"/g,']')
		.replace(/\"\{/g, '{')
		.replace(/\}\"/g,'}');
};

//////////////////////////////////////////////////////////////

GLOBALS.trace = console.log.bind(console);
GLOBALS.traceObj = function(o) {
	var output = _.keys(o).sort();
	trace(output);
	return output;
};

if(isNode()) {
	GLOBALS.traceClear = function() { process.stdout.write('\033c'); };
	GLOBALS.traceError = function(err) { trace(err.toString().red); };
} else {
	GLOBALS.traceClear = function() { console.clear && console.clear(); };
	GLOBALS.traceError = function(err) { console.error(err); };
}