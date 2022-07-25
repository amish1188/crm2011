const { merge } = require("webpack-merge");
const commonConfig = require("./webpack.common");

const devConfig = {
    mode: "development",
    devtool: "source-map",
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [
                    "style-loader",
                    {loader: 'css-loader', options: { sourceMap: true }}
                ]
            },
            {
                test: /\.(png|jpe?g|gif)$/i,
                loader: 'file-loader',
                options: {
                    outputPath: 'images',
                },
            },
            {
                test: /\.(js|jsx)$/,
                exclude: /(node_modules)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env', '@babel/preset-react']
                    }
                }
            }        ]
    }
};

module.exports = merge(commonConfig, devConfig);