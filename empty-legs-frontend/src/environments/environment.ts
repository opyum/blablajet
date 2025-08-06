export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  auth: {
    clientId: 'empty-legs-frontend',
    authority: 'https://localhost:5001',
    responseType: 'code',
    scope: 'openid profile email api1'
  },
  maps: {
    googleMapsApiKey: 'YOUR_GOOGLE_MAPS_API_KEY'
  },
  features: {
    enableAnalytics: false,
    enableNotifications: true,
    enableDebugMode: true
  }
};