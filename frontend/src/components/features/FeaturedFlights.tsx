import { Plane, Clock, MapPin, Users } from 'lucide-react'

interface Flight {
  id: string
  flightNumber: string
  departureAirport: {
    code: string
    name: string
    city: string
  }
  arrivalAirport: {
    code: string
    name: string
    city: string
  }
  departureTime: string
  arrivalTime: string
  currentPrice: number
  availableSeats: number
  aircraft: {
    model: string
    type: string
  }
  company: {
    name: string
    logoUrl: string
  }
}

// Données d'exemple pour les vols en vedette
const featuredFlights: Flight[] = [
  {
    id: '1',
    flightNumber: 'EL001',
    departureAirport: { code: 'CDG', name: 'Charles de Gaulle', city: 'Paris' },
    arrivalAirport: { code: 'NCE', name: 'Côte d\'Azur', city: 'Nice' },
    departureTime: '2024-03-15T14:30:00Z',
    arrivalTime: '2024-03-15T16:00:00Z',
    currentPrice: 1200,
    availableSeats: 6,
    aircraft: { model: 'Citation CJ3+', type: 'Light Jet' },
    company: { name: 'AirLuxe', logoUrl: '/api/placeholder/40/40' }
  },
  {
    id: '2',
    flightNumber: 'EL002',
    departureAirport: { code: 'LHR', name: 'Heathrow', city: 'Londres' },
    arrivalAirport: { code: 'GVA', name: 'Genève', city: 'Genève' },
    departureTime: '2024-03-16T10:15:00Z',
    arrivalTime: '2024-03-16T13:45:00Z',
    currentPrice: 2100,
    availableSeats: 8,
    aircraft: { model: 'Falcon 2000', type: 'Mid Size' },
    company: { name: 'SwissJet', logoUrl: '/api/placeholder/40/40' }
  },
  {
    id: '3',
    flightNumber: 'EL003',
    departureAirport: { code: 'MAD', name: 'Barajas', city: 'Madrid' },
    arrivalAirport: { code: 'IBZ', name: 'Ibiza', city: 'Ibiza' },
    departureTime: '2024-03-17T16:20:00Z',
    arrivalTime: '2024-03-17T17:30:00Z',
    currentPrice: 950,
    availableSeats: 4,
    aircraft: { model: 'King Air 350', type: 'Turboprop' },
    company: { name: 'MediterraneanAir', logoUrl: '/api/placeholder/40/40' }
  }
]

function FlightCard({ flight }: { flight: Flight }) {
  const formatTime = (dateString: string) => {
    return new Date(dateString).toLocaleTimeString('fr-FR', {
      hour: '2-digit',
      minute: '2-digit'
    })
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('fr-FR', {
      day: 'numeric',
      month: 'short'
    })
  }

  return (
    <div className="bg-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 overflow-hidden group">
      <div className="p-6">
        {/* Header avec compagnie */}
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center space-x-3">
            <img 
              src={flight.company.logoUrl} 
              alt={flight.company.name}
              className="w-10 h-10 rounded-full"
            />
            <div>
              <p className="font-semibold text-gray-900">{flight.company.name}</p>
              <p className="text-sm text-gray-500">{flight.flightNumber}</p>
            </div>
          </div>
          <div className="text-right">
            <p className="text-2xl font-bold text-blue-600">{flight.currentPrice.toLocaleString()}€</p>
            <p className="text-sm text-gray-500">par passager</p>
          </div>
        </div>

        {/* Itinéraire */}
        <div className="flex items-center justify-between mb-4">
          <div className="text-center">
            <p className="text-2xl font-bold text-gray-900">{flight.departureAirport.code}</p>
            <p className="text-sm text-gray-600">{flight.departureAirport.city}</p>
            <p className="text-lg font-semibold text-gray-800">{formatTime(flight.departureTime)}</p>
            <p className="text-xs text-gray-500">{formatDate(flight.departureTime)}</p>
          </div>
          
          <div className="flex-1 px-4">
            <div className="relative">
              <div className="h-0.5 bg-gray-300"></div>
              <Plane className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-blue-600 bg-white p-1 w-8 h-8" />
            </div>
          </div>
          
          <div className="text-center">
            <p className="text-2xl font-bold text-gray-900">{flight.arrivalAirport.code}</p>
            <p className="text-sm text-gray-600">{flight.arrivalAirport.city}</p>
            <p className="text-lg font-semibold text-gray-800">{formatTime(flight.arrivalTime)}</p>
            <p className="text-xs text-gray-500">{formatDate(flight.arrivalTime)}</p>
          </div>
        </div>

        {/* Détails de l'avion */}
        <div className="flex items-center justify-between mb-4 p-3 bg-gray-50 rounded-lg">
          <div className="flex items-center space-x-2">
            <Plane className="w-4 h-4 text-gray-600" />
            <span className="text-sm text-gray-700">{flight.aircraft.model}</span>
          </div>
          <div className="flex items-center space-x-2">
            <Users className="w-4 h-4 text-gray-600" />
            <span className="text-sm text-gray-700">{flight.availableSeats} places</span>
          </div>
        </div>

        {/* Bouton de réservation */}
        <button className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-4 rounded-lg transition-colors duration-200">
          Réserver maintenant
        </button>
      </div>
    </div>
  )
}

export function FeaturedFlights() {
  return (
    <section className="py-16 bg-gray-50">
      <div className="container mx-auto px-4">
        <div className="text-center mb-12">
          <h2 className="text-4xl font-bold text-gray-900 mb-4">
            Vols à vide du moment
          </h2>
          <p className="text-xl text-gray-600 max-w-3xl mx-auto">
            Découvrez nos meilleures offres de jets privés disponibles. 
            Réservez rapidement pour bénéficier de tarifs exceptionnels.
          </p>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {featuredFlights.map((flight) => (
            <FlightCard key={flight.id} flight={flight} />
          ))}
        </div>
        
        <div className="text-center mt-12">
          <button className="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-8 rounded-lg transition-colors duration-200">
            Voir tous les vols
          </button>
        </div>
      </div>
    </section>
  )
}