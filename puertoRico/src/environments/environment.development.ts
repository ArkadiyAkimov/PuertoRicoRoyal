export const environment = {
    production:false,
    apiUrl: (() => {
        const url = new URL(window.location.href);
        if (url.hostname === 'localhost') {
          return 'http://localhost:7168/api';
        } else if (url.hostname === '192.168.1.110') {
          return 'http://192.168.1.110:7168/api';
        } else {
          return 'https://example.com/api';
        }
      })()
    };