/**
 * Created by Chamberlain on 01/03/2017.
 */


require('./globals');

const server = require('http').createServer();
const io = require('socket.io')(server);
const port = 9999;

io.on('connection', function(client){
  client.on('event', function(data){
	  trace("Got data! id: " +client.id + " : " + data);
  });
  client.on('disconnect', function(){
	  trace("disconnected " + client.id);
  });
});

trace("Listening on port: " + port)
server.listen(port);