var path = require('path');

module.exports = {
  entry: './NodeScripts/index.js',
  output: {
    path: path.resolve(__dirname, 'Scripts'),
    filename: 'lendinglibrary.js',
    library: 'lendingLibrary'
  },
  target: 'web',
  node: {
    fs: 'empty',
    tls: 'empty',
    net: 'empty'
  }
};

