import { Star, Quote } from 'lucide-react'

interface Testimonial {
  id: string
  name: string
  role: string
  company?: string
  avatar: string
  rating: number
  comment: string
  flight: {
    route: string
    date: string
  }
}

const testimonials: Testimonial[] = [
  {
    id: '1',
    name: 'Sophie Laurent',
    role: 'Directrice Marketing',
    company: 'TechCorp',
    avatar: '/api/placeholder/60/60',
    rating: 5,
    comment: "Service exceptionnel ! J'ai pu me rendre à Londres en dernière minute pour un rendez-vous crucial. L'équipe était professionnelle et l'avion impeccable.",
    flight: {
      route: 'Paris → Londres',
      date: 'Février 2024'
    }
  },
  {
    id: '2',
    name: 'Marc Dubois',
    role: 'Entrepreneur',
    avatar: '/api/placeholder/60/60',
    rating: 5,
    comment: "Incroyable rapport qualité-prix ! J'ai économisé plus de 3000€ par rapport à une location classique. Je recommande vivement Empty Legs.",
    flight: {
      route: 'Nice → Monaco',
      date: 'Mars 2024'
    }
  },
  {
    id: '3',
    name: 'Isabella Rodriguez',
    role: 'Architecte',
    company: 'Design Studio',
    avatar: '/api/placeholder/60/60',
    rating: 5,
    comment: "L'expérience était parfaite du début à la fin. Réservation simple, équipe accueillante et vol ponctuel. Une nouvelle façon de voyager !",
    flight: {
      route: 'Madrid → Ibiza',
      date: 'Janvier 2024'
    }
  },
  {
    id: '4',
    name: 'James Wilson',
    role: 'Investisseur',
    avatar: '/api/placeholder/60/60',
    rating: 4,
    comment: "Très satisfait de mon vol. La plateforme est intuitive et le service clientèle réactif. Je vais certainement réutiliser ce service.",
    flight: {
      route: 'Genève → Milan',
      date: 'Décembre 2023'
    }
  }
]

function TestimonialCard({ testimonial }: { testimonial: Testimonial }) {
  return (
    <div className="bg-white rounded-xl shadow-lg p-6 h-full flex flex-col">
      {/* Header avec citation */}
      <div className="flex items-start space-x-3 mb-4">
        <Quote className="w-8 h-8 text-blue-600 flex-shrink-0 mt-1" />
        <p className="text-gray-700 leading-relaxed flex-grow">
          "{testimonial.comment}"
        </p>
      </div>

      {/* Rating */}
      <div className="flex items-center space-x-1 mb-4">
        {[...Array(5)].map((_, i) => (
          <Star
            key={i}
            className={`w-5 h-5 ${
              i < testimonial.rating
                ? 'text-yellow-400 fill-current'
                : 'text-gray-300'
            }`}
          />
        ))}
        <span className="text-sm text-gray-600 ml-2">({testimonial.rating}/5)</span>
      </div>

      {/* Flight info */}
      <div className="bg-blue-50 rounded-lg p-3 mb-4">
        <p className="text-sm text-blue-800 font-medium">{testimonial.flight.route}</p>
        <p className="text-xs text-blue-600">{testimonial.flight.date}</p>
      </div>

      {/* User info */}
      <div className="flex items-center space-x-3 mt-auto">
        <img
          src={testimonial.avatar}
          alt={testimonial.name}
          className="w-12 h-12 rounded-full object-cover"
        />
        <div>
          <p className="font-semibold text-gray-900">{testimonial.name}</p>
          <p className="text-sm text-gray-600">
            {testimonial.role}
            {testimonial.company && (
              <span className="text-gray-500"> • {testimonial.company}</span>
            )}
          </p>
        </div>
      </div>
    </div>
  )
}

export function Testimonials() {
  return (
    <section className="py-16 bg-gradient-to-br from-gray-50 to-blue-50">
      <div className="container mx-auto px-4">
        <div className="text-center mb-16">
          <h2 className="text-4xl font-bold text-gray-900 mb-4">
            Ce que disent nos clients
          </h2>
          <p className="text-xl text-gray-600 max-w-3xl mx-auto">
            Découvrez les expériences de nos passagers qui ont choisi Empty Legs
            pour leurs voyages en jet privé.
          </p>
        </div>

        {/* Grille de témoignages */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
          {testimonials.map((testimonial) => (
            <TestimonialCard key={testimonial.id} testimonial={testimonial} />
          ))}
        </div>

        {/* Statistiques de satisfaction */}
        <div className="bg-white rounded-2xl shadow-lg p-8">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 text-center">
            <div>
              <div className="text-3xl font-bold text-blue-600 mb-2">4.8/5</div>
              <p className="text-gray-600">Note moyenne</p>
              <div className="flex justify-center items-center space-x-1 mt-2">
                {[...Array(5)].map((_, i) => (
                  <Star
                    key={i}
                    className={`w-4 h-4 ${
                      i < 5 ? 'text-yellow-400 fill-current' : 'text-gray-300'
                    }`}
                  />
                ))}
              </div>
            </div>
            <div>
              <div className="text-3xl font-bold text-green-600 mb-2">500+</div>
              <p className="text-gray-600">Vols réalisés</p>
            </div>
            <div>
              <div className="text-3xl font-bold text-purple-600 mb-2">98%</div>
              <p className="text-gray-600">Clients satisfaits</p>
            </div>
            <div>
              <div className="text-3xl font-bold text-orange-600 mb-2">24h/7</div>
              <p className="text-gray-600">Support client</p>
            </div>
          </div>
        </div>

        {/* Call to action */}
        <div className="text-center mt-12">
          <p className="text-lg text-gray-600 mb-6">
            Rejoignez nos clients satisfaits et découvrez le luxe abordable
          </p>
          <button className="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-8 rounded-lg transition-colors duration-200">
            Rechercher un vol
          </button>
        </div>
      </div>
    </section>
  )
}