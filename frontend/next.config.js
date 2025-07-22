/** @type {import('next').NextConfig} */
const nextConfig = {

  images: {
    domains: [
      'localhost',
      'emptylegs-api.azurewebsites.net',
      'blob.core.windows.net',
    ],
  },
  env: {
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL,
    NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY: process.env.NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY,
    NEXT_PUBLIC_GOOGLE_MAPS_API_KEY: process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY,
  },
  typescript: {
    // Only run type-checking in CI
    ignoreBuildErrors: process.env.NODE_ENV === 'production',
  },
  eslint: {
    // Only run ESLint in CI
    ignoreDuringBuilds: process.env.NODE_ENV === 'production',
  },
}

module.exports = nextConfig