import App from './vue/app.vue'

$(document).ready(function() {
	BIGP.app = new Vue({
		el: '#app',
		data: {
			title: "This is the title!"
		},

		components: {App}
	});

	setTimeout(() => BIGP.app.$forceUpdate(), 0);
});
