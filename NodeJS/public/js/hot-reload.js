/**
 * Created by Chamberlain on 11/03/2017.
 */

BIGP.io = io();

BIGP.io.on('webpack', function(data) {
	window.location.reload(true);
});