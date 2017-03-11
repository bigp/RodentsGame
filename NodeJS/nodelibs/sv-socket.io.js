/**
 * Created by Chamberlain on 10/03/2017.
 */
module.exports = function(BIGP) {

	var webClients = (BIGP.webClients = BIGP.webClients || []);

	BIGP.io.on('connection', function(client){
		trace((" >> " + client.id).yellow);

		client.on('event', function(data){
			trace("Got data! id: " +client.id + " : " + data);
		});

		client.on('echo', function(data){
			client.emit("echo", "Server echo: " + data);
		});

		client.on('web-client', function(data) {
			webClients.push(client);
		});

		client.on('update', function(data) {

		});

		client.on('disconnect', function(){
			trace(("   << " + client.id).red);
		});
	});

	BIGP.beep = function() {
		trace("!!!!!!!!!!!!!!!!!!!!!!!!".redBG.white);
		BIGP.io.emit('beep');
	};
};