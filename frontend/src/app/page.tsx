import { Suspense } from 'react'
import { Hero } from '../components/layout/Hero'
import { SearchSection } from '../components/features/SearchSection'
import { FeaturedFlights } from '../components/features/FeaturedFlights'
import { HowItWorks } from '../components/features/HowItWorks'
import { Testimonials } from '../components/features/Testimonials'
import { Header } from '../components/layout/Header'
import { Footer } from '../components/layout/Footer'

export default function HomePage() {
  return (
    <main className="min-h-screen">
      <Header />
      
      {/* Hero Section with Search */}
      <Hero />
      
      {/* Search Section */}
      <SearchSection />
      
      {/* Featured Flights */}
      <Suspense fallback={<div className="container py-12">Chargement des vols...</div>}>
        <FeaturedFlights />
      </Suspense>
      
      {/* How It Works */}
      <HowItWorks />
      
      {/* Testimonials */}
      <Testimonials />
      
      <Footer />
    </main>
  )
}