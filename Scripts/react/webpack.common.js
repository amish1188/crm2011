const path = require('path');

const config = {
    entry: './index.js',
    devServer: {
        static: "./dist"
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'bundle.js',
        assetModuleFilename: 'images/[hash][ext][query]'
    },
    resolve: {
        extensions: ['.js', '.jsx'],
    },
}

module.exports = config;