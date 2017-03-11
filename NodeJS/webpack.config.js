module.exports = {
	entry: {
		main: './src/main'
	},

	output: {
		path: './public/js',
		filename: '[name].bundle.js',
		chunkFilename: '[id].bundle.js'
	},

	module: {
		rules: [
			{test: /\.js$/, use: 'babel-loader', exclude: /node_modules/},
			{test: /\.vue$/, loader: 'vue-loader', exclude: /node_modules/}
		]
	}
};