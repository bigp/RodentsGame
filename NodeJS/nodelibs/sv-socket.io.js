/**
 * Created by Chamberlain on 10/03/2017.
 */
module.exports = function(BIGP) {

	BIGP.io.on('connection', function(client){
		trace((" >> " + client.id).yellow);

		client.on('event', function(data){
			trace("Got data! id: " +client.id + " : " + data);
		});

		client.on('echo', function(data){
			client.emit("echo", data);
		});

		client.on('disconnect', function(){
			trace(("   << " + client.id).red);
		});

		//Extend the socket with game-specific features:
		require('./game/sv-game-sockets')(BIGP, client);
	});

	BIGP.beep = function() {
		trace("!!!!!!!!!!!!!!!!!!!!!!!!".redBG.white);
		BIGP.io.emit('beep');
	};
};