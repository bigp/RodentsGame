/**
 * Created by Chamberlain on 01/03/2017.
 */


require('./common/globals');

const port = process.env.PORT || 9999;

BIGP.server.listen(port, function() {
  BIGP.emit('ready');
  trace(("Listening on port: " + port).yellowBG.black);
});