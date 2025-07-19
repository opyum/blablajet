import { ArrowRight, Plane } from 'lucide-react'
import Link from 'next/link'

export function Hero() {
  return (
    <section className="relative min-h-screen flex items-center justify-center overflow-hidden">
      {/* Background with gradient overlay */}
      <div className="absolute inset-0 gradient-hero"></div>
      <div className="absolute inset-0 bg-black/20"></div>
      
      {/* Background pattern */}
      <div className="absolute inset-0 opacity-10">
        <svg className="w-full h-full" viewBox="0 0 100 100" preserveAspectRatio="none">
          <pattern id="plane-pattern" x="0" y="0" width="20" height="20" patternUnits="userSpaceOnUse">
            <Plane className="w-4 h-4 text-white opacity-30" />
          </pattern>
          <rect width="100%" height="100%" fill="url(#plane-pattern)" />
        </svg>
      </div>

      {/* Content */}
      <div className="relative z-10 container text-center text-white">
        <div className="max-w-4xl mx-auto">
          {/* Badge */}
          <div className="inline-flex items-center px-4 py-2 mb-6 bg-white/10 backdrop-blur-sm rounded-full border border-white/20">
            <span className="text-sm font-medium">✈️ Plateforme #1 des vols à vide</span>
          </div>

          {/* Main heading */}
          <h1 className="text-4xl md:text-6xl lg:text-7xl font-bold mb-6 leading-tight">
            Voyagez en 
            <span className="block bg-gradient-to-r from-yellow-400 to-orange-400 bg-clip-text text-transparent">
              Jet Privé
            </span>
            à prix accessible
          </h1>

          {/* Subtitle */}
          <p className="text-lg md:text-xl mb-8 text-gray-100 max-w-2xl mx-auto leading-relaxed">
            Découvrez les vols à vide des compagnies d&apos;aviation privée et profitez 
            de tarifs exceptionnels pour voyager dans le luxe et le confort.
          </p>

          {/* Stats */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10 max-w-2xl mx-auto">
            <div className="text-center">
              <div className="text-2xl md:text-3xl font-bold mb-1">500+</div>
              <div className="text-sm text-gray-200">Vols disponibles</div>
            </div>
            <div className="text-center">
              <div className="text-2xl md:text-3xl font-bold mb-1">150+</div>
              <div className="text-sm text-gray-200">Compagnies partenaires</div>
            </div>
            <div className="text-center">
              <div className="text-2xl md:text-3xl font-bold mb-1">-70%</div>
              <div className="text-sm text-gray-200">Économies moyennes</div>
            </div>
          </div>

          {/* CTA Buttons */}
          <div className="flex flex-col sm:flex-row gap-4 justify-center items-center">
            <Link 
              href="/search" 
              className="btn btn-lg bg-white text-primary hover:bg-gray-100 min-w-[200px] group"
            >
              Rechercher un vol
              <ArrowRight className="w-5 h-5 ml-2 group-hover:translate-x-1 transition-transform" />
            </Link>
            <Link 
              href="/how-it-works" 
              className="btn btn-lg btn-outline border-white text-white hover:bg-white hover:text-primary min-w-[200px]"
            >
              Comment ça marche
            </Link>
          </div>

          {/* Trust indicators */}
          <div className="mt-12 pt-8 border-t border-white/20">
            <p className="text-sm text-gray-200 mb-4">Ils nous font confiance</p>
            <div className="flex flex-wrap justify-center items-center gap-8 opacity-60">
              <div className="text-white font-semibold">AirFrance</div>
              <div className="text-white font-semibold">NetJets</div>
              <div className="text-white font-semibold">VistaJet</div>
              <div className="text-white font-semibold">Flexjet</div>
            </div>
          </div>
        </div>
      </div>

      {/* Scroll indicator */}
      <div className="absolute bottom-8 left-1/2 transform -translate-x-1/2">
        <div className="w-6 h-10 border-2 border-white/30 rounded-full flex justify-center">
          <div className="w-1 h-3 bg-white/60 rounded-full mt-2 animate-bounce"></div>
        </div>
      </div>
    </section>
  )
}