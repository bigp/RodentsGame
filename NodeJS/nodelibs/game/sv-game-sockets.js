/**
 * Created by Chamberlain on 11/03/2017.
 */

module.exports = function(BIGP, client) {
	trace("GAMEZZZZZZZZ".green);

	var webClients = (BIGP.webClients = BIGP.webClients || []);

	var worldBounds;

	const EVENTS = {
		WEB_CLIENT: 'web-client',
		WORLD_BOUNDS: 'world-bounds',
		DISCONNECT: 'disconnect'
	};

	function sendToWeb(event, data) {
		webClients.forEach(sock => sock.emit(event, data));
	}

	client.on(EVENTS.WEB_CLIENT, function(data) {
		trace('web client added!'.green);
		webClients.push(client);

		if(worldBounds) {
			client.emit(EVENTS.WORLD_BOUNDS, worldBounds);
		}
	});

	client.on(EVENTS.DISCONNECT, function() {
		var id = webClients.remove(client);
		if(id>-1) {
			trace('web client removed!'.red);
		}
	});

	/////////////////////////////////////////////////

	client.on(EVENTS.WORLD_BOUNDS, function(data) {
		worldBounds = data;

		trace("worldBounds: " + worldBounds);
		trace(`Sending "${EVENTS.WORLD_BOUNDS}" to webclients: ` + webClients.length);
		sendToWeb(EVENTS.WORLD_BOUNDS, data);
	});
};