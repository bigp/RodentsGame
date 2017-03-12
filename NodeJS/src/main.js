import App from './vue/app.vue'
import VueDirectives from './js/vue-directives'

$(document).ready(function() {
	BIGP.io = io();

	BIGP.io.on('webpack', function(data) {
		window.location.reload(true);
	});

	BIGP.app = new Vue({
		el: '#app',
		data: {
			title: "This is the title!"
		},

		components: {App}
	});

	setTimeout(() => BIGP.app.$forceUpdate(), 0);
});
