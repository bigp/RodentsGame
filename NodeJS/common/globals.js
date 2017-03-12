const _ = global._ = require('lodash');
const mkdirp = global.mkdirp = require('mkdirp');
const EventEmitter = require('events');
class NamespaceEmitter extends EventEmitter {}

const BIGP = global.BIGP = new NamespaceEmitter();
require('./shared/extensions');
require('./helpers')(BIGP);
require('colors');
require('dotenv').config({path: '.private/config.env'});

BIGP.IS_DEV = _.isTruthy(process.env.IS_DEV);
BIGP.IS_WEBPACK = _.isTruthy(process.env.IS_WEBPACK);

const _traceError = global.traceError;
global.traceError = function(msg) {
	if(msg==null) return;
	_traceError(msg.toString().red);
};

traceClear();

//Setup typical Express + Socket.IO Networking:
BIGP.express = require('express');
BIGP.app = BIGP.express();
BIGP.server = require('http').createServer(BIGP.app);
BIGP.io = require('socket.io')(BIGP.server);

BIGP.__args = require('yargs').argv;
BIGP.__dir = process.cwd().fixSlashes();
BIGP.__public = BIGP.__dir + "/public";
BIGP.__nodelib = BIGP.__dir + "/nodelibs";
BIGP.__common = BIGP.__dir + "/common";
BIGP.__shared = BIGP.__dir + "/common/shared";
BIGP.__src = BIGP.__dir + "/src";

trace(BIGP.__public.cyan);

BIGP.loadModules( BIGP.__dir + '/nodelibs/', BIGP );
