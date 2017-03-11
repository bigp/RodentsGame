/**
 * Created by Chamberlain on 10/03/2017.
 */
const webpack = require('webpack');
const config = require('../webpack.config.js');

module.exports = function(BIGP) {
	config.context = BIGP.__dir;
	trace(config.context);

	const compiler = webpack(config);

	BIGP.on('file-changed', () => {
		trace("Webpacking...".yellow);
		compiler.run((err) => {
			if(err) return traceError(err);

			BIGP.emit('webpack');
			BIGP.io.emit('webpack');

			traceClear();

			setTimeout(() => {
				trace("Webpack Completed!".green);
			}, 250);

		});
	});
};