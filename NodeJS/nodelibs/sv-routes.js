/**
 * Created by Chamberlain on 10/03/2017.
 */
module.exports = function(BIGP) {
	const app = BIGP.app;
	const errorHeader = '<h1>Something broke!</h1><br/>';

	function status500(res, msg) {
		res.status(500).send(errorHeader + msg);
	}

	app.use(function(req, res, next) {
		var url = req.url;
		if(url.endsWith('/') && url.length > 1) {
			trace("Redirect trailing slash!");
			res.redirect(302, url.slice(0, -1));
		} else
			next();
	});

	app.use('/', BIGP.express.static(BIGP.__public));
	app.use('/js', BIGP.express.static(BIGP.__shared));

	//Error Handler:
	app.use(function(err, req, res, next) {
		if(err) {
			traceError(err);
		}

		status500(res, err.message);
	});
};