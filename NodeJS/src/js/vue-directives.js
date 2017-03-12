/**
 * Created by Chamberlain on 11/03/2017.
 */
Vue.directive('io', {
	bind(el, binding, vnode) {
		var ioEvent = _.keys(binding.modifiers)[0];
		BIGP.io.on(ioEvent, function(data) {
			if(_.isString(data) && data.startsWith('{')) {
				data = JSON.parse(data);
			} else {
				trace("Not json?");
			}

			var _value = binding.value;
			if(_.isFunction(_value)) {
				_value(data);
			} else {
				traceError("Socket.IO Vue directive's binding is not a function :(");
			}
		});
	}
});