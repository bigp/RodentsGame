const _ = global._ = require('lodash');
const mkdirp = global.mkdirp = require('mkdirp');

const BIGP = global.BIGP = {};
require('./extensions');
require('./helpers')(BIGP);
require('colors');

BIGP.__dir = process.cwd().fixSlashes();
BIGP.__args = require('yargs').argv;