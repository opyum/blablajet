import { Search, Calendar, CreditCard, Plane } from 'lucide-react'

const steps = [
  {
    icon: Search,
    title: "Recherchez",
    description: "Saisissez votre destination et vos dates pour découvrir les vols à vide disponibles",
    color: "bg-blue-100 text-blue-600"
  },
  {
    icon: Calendar,
    title: "Sélectionnez",
    description: "Comparez les options, horaires et prix pour choisir le vol qui vous convient",
    color: "bg-green-100 text-green-600"
  },
  {
    icon: CreditCard,
    title: "Payez",
    description: "Réservez en toute sécurité avec notre système de paiement sécurisé Stripe",
    color: "bg-purple-100 text-purple-600"
  },
  {
    icon: Plane,
    title: "Voyagez",
    description: "Présentez-vous à l'aéroport et profitez de votre vol en jet privé à prix réduit",
    color: "bg-orange-100 text-orange-600"
  }
]

export function HowItWorks() {
  return (
    <section className="py-16 bg-white">
      <div className="container mx-auto px-4">
        <div className="text-center mb-16">
          <h2 className="text-4xl font-bold text-gray-900 mb-4">
            Comment ça marche ?
          </h2>
          <p className="text-xl text-gray-600 max-w-3xl mx-auto">
            Réservez votre vol en jet privé en 4 étapes simples. 
            Une expérience de luxe à portée de main.
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          {steps.map((step, index) => {
            const IconComponent = step.icon
            return (
              <div key={index} className="text-center">
                {/* Icône avec numérotation */}
                <div className="relative mb-6">
                  <div className={`w-20 h-20 mx-auto rounded-full ${step.color} flex items-center justify-center mb-4`}>
                    <IconComponent className="w-8 h-8" />
                  </div>
                  <div className="absolute -top-2 -right-2 w-8 h-8 bg-gray-900 text-white rounded-full flex items-center justify-center text-sm font-bold">
                    {index + 1}
                  </div>
                </div>

                {/* Contenu */}
                <h3 className="text-xl font-semibold text-gray-900 mb-3">
                  {step.title}
                </h3>
                <p className="text-gray-600 leading-relaxed">
                  {step.description}
                </p>

                {/* Connecteur (sauf pour le dernier élément) */}
                {index < steps.length - 1 && (
                  <div className="hidden lg:block absolute top-10 left-1/2 w-full h-0.5 bg-gray-200 transform translate-x-1/2 z-0" />
                )}
              </div>
            )
          })}
        </div>

        {/* Section avantages */}
        <div className="mt-16 bg-gradient-to-r from-blue-50 to-purple-50 rounded-2xl p-8">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="text-center">
              <div className="w-12 h-12 bg-blue-600 text-white rounded-lg flex items-center justify-center mx-auto mb-3">
                ⚡
              </div>
              <h4 className="font-semibold text-gray-900 mb-2">Réservation instantanée</h4>
              <p className="text-sm text-gray-600">Confirmation immédiate de votre vol</p>
            </div>
            <div className="text-center">
              <div className="w-12 h-12 bg-green-600 text-white rounded-lg flex items-center justify-center mx-auto mb-3">
                💰
              </div>
              <h4 className="font-semibold text-gray-900 mb-2">Prix avantageux</h4>
              <p className="text-sm text-gray-600">Jusqu'à 70% moins cher qu'un vol charter</p>
            </div>
            <div className="text-center">
              <div className="w-12 h-12 bg-purple-600 text-white rounded-lg flex items-center justify-center mx-auto mb-3">
                🛡️
              </div>
              <h4 className="font-semibold text-gray-900 mb-2">Service premium</h4>
              <p className="text-sm text-gray-600">Expérience haut de gamme garantie</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}