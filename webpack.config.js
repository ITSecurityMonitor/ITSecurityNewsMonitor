const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const bundleFileName = 'ITSecurityNewsMonitor';
const dirName = 'ITSecurityNewsMonitor/wwwroot/dist';

module.exports = (env, argv) => {
    return {
        mode: argv.mode === "production" ? "production" : "development",
        entry: ['./ITSecurityNewsMonitor/wwwroot/js/site.js', './ITSecurityNewsMonitor/wwwroot/css/site.scss', './ITSecurityNewsMonitor/wwwroot/css/test.scss'],
        output: {
            filename: bundleFileName + '.js',
            path: path.resolve(__dirname, dirName)
        },
        module: {
            rules: [{
                test: /\.s[ac]ss$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    'postcss-loader',
                    "sass-loader",
                ]
            }]
        },
        plugins: [
            new MiniCssExtractPlugin({
                filename: bundleFileName + '.css'
            })
        ]
    };
};