const https = require('https');

export default {
    importScript(url) {
      return new Promise((resolve, reject) => {
        https.get(url, response => {
          let data = '';
          response.on('data', chunk => {
            data += chunk;
          });
          response.on('end', () => {
            try {
              const script = eval(data);
              resolve(script);
            } catch (error) {
              reject(error);
            }
          });
          response.on('error', error => {
            reject(error);
          });
        });
      });
    }
}