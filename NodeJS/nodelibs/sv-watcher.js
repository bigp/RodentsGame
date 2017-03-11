const chokidar = require('chokidar');

module.exports = function(BIGP) {
	if(!BIGP.IS_DEV) {
		return trace("Skip Watcher since we're running PRODUCTION".yellow);
	}

	var timeoutID = -1;

	var requiresRestart = false;

	// One-liner for current directory, ignores .dotfiles
	chokidar.watch([BIGP.__nodelib, BIGP.__public, BIGP.__src], {ignored: /[\/\\]\./}).on('all', (event, path) => {
		path = path.fixSlashes();

		if (event.has('add') ||
			path.endsWith('.ts') ||
			path.endsWith('.less')) return;

		if(path.has('/nodelib')) {
			BIGP.beep();
			if(requiresRestart) return;
			requiresRestart = true;
			return traceError("CHANGED server-side node JS file:\n  " + path);
		}

		if(!BIGP.io) return;

		fileChanged(path);
	});

	function fileChanged(path, time) {
		if(requiresRestart) return;
		if(!time) time = process.env.WATCHER_DELAY || 500;
		BIGP.beep();

		if(timeoutID>-1) clearTimeout(timeoutID);

		trace("watcher delay: " + time);
		timeoutID = setTimeout(() => {
			timeoutID = -1;

			BIGP.emit('file-changed', path);
			BIGP.io.emit('file-changed', path);
		}, time);
	}

	//First initial REFRESH call:
	process.nextTick(() => {
		fileChanged('');
	});
};