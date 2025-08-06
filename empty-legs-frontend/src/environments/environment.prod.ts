export const environment = {
  production: true,
  apiUrl: 'https://api.emptylegs.com/api',
  auth: {
    clientId: 'empty-legs-frontend',
    authority: 'https://auth.emptylegs.com',
    responseType: 'code',
    scope: 'openid profile email api1'
  },
  maps: {
    googleMapsApiKey: 'YOUR_PRODUCTION_GOOGLE_MAPS_API_KEY'
  },
  features: {
    enableAnalytics: true,
    enableNotifications: true,
    enableDebugMode: false
  }
};