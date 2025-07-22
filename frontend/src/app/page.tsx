import { Suspense } from 'react'
import { Hero } from '../components/layout/Hero'
import { UniversalSearch } from '../components/features/UniversalSearch'
import { InteractiveMap } from '../components/features/InteractiveMap'
import { FeaturedFlights } from '../components/features/FeaturedFlights'
import { HowItWorks } from '../components/features/HowItWorks'
import { Testimonials } from '../components/features/Testimonials'
import { Header } from '../components/layout/Header'
import { Footer } from '../components/layout/Footer'

// Sample data for the map
const sampleMapItems = [
  {
    id: '1',
    type: 'flight' as const,
    title: 'Cessna Citation CJ3+',
    location: { lat: 48.8566, lng: 2.3522 },
    price: 8500,
    rating: 4.8,
    image: '/api/placeholder/300/200',
    description: 'Vol Paris → Nice en jet privé',
    luxuryLevel: 'luxury'
  },
  {
    id: '2',
    type: 'yacht' as const,
    title: 'Sunseeker 86',
    location: { lat: 43.7384, lng: 7.4246 },
    price: 12000,
    rating: 4.9,
    image: '/api/placeholder/300/200',
    description: 'Yacht de luxe à Monaco',
    luxuryLevel: 'ultra-luxury'
  },
  {
    id: '3',
    type: 'car' as const,
    title: 'Ferrari 488 Spider',
    location: { lat: 43.7034, lng: 7.4197 },
    price: 1200,
    rating: 4.7,
    image: '/api/placeholder/300/200',
    description: 'Supercar de location à Nice',
    luxuryLevel: 'exclusive'
  },
  {
    id: '4',
    type: 'hotel' as const,
    title: 'Hôtel Martinez Cannes',
    location: { lat: 43.5514, lng: 7.0128 },
    price: 850,
    rating: 4.6,
    image: '/api/placeholder/300/200',
    description: 'Hôtel 5 étoiles avec vue mer',
    luxuryLevel: 'luxury'
  }
]

export default function HomePage() {
  return (
    <main className="min-h-screen">
      <Header />
      
      {/* Hero Section */}
      <Hero />
      
      {/* Universal Search Section */}
      <section className="py-16 bg-gradient-to-br from-blue-50 to-purple-50">
        <div className="container">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold text-gray-900 mb-4">
              Trouvez votre expérience de luxe
            </h2>
            <p className="text-xl text-gray-600 max-w-3xl mx-auto">
              Jets privés, yachts exclusifs, voitures de prestige et hôtels d'exception. 
              Réservez en quelques clics votre prochaine aventure ultra-premium.
            </p>
          </div>
          <UniversalSearch />
        </div>
      </section>
      
      {/* Interactive Map Section */}
      <section className="py-16 bg-white">
        <div className="container">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold text-gray-900 mb-4">
              Explorez sur la carte
            </h2>
            <p className="text-xl text-gray-600 max-w-3xl mx-auto">
              Découvrez nos services de luxe géolocalisés et trouvez 
              l'expérience parfaite près de vous.
            </p>
          </div>
          <div className="rounded-2xl overflow-hidden shadow-2xl">
            <InteractiveMap 
              items={sampleMapItems}
              center={{ lat: 46.2276, lng: 2.2137 }}
              zoom={6}
            />
          </div>
        </div>
      </section>
      
      {/* Featured Services */}
      <Suspense fallback={<div className="container py-12">Chargement des services...</div>}>
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