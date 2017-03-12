/**
 * Created by Chamberlain on 10/03/2017.
 */
const webpack = require('webpack');
const config = require('../webpack.config.js');

module.exports = function(BIGP) {
	if(!BIGP.IS_WEBPACK) {
		return traceError("Not running webpack. Enable in ENV file by setting: " + ('IS_WEBPACK'.green));
	}
	config.context = BIGP.__dir;
	trace(config.context);

	const compiler = webpack(config);

	BIGP.on('file-changed', () => {
		trace("Webpacking...".yellow);
		compiler.run((err) => {
			if(err) return traceError(err);

			traceClear();

			setTimeout(() => {
				trace("Webpack Completed!".green);
				BIGP.emit('webpack');
				BIGP.io.emit('webpack');
			}, 250);

		});
	});
};