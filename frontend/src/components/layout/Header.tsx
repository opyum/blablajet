'use client'

import { useState } from 'react'
import Link from 'next/link'
import { Plane, Menu, X, User } from 'lucide-react'

export function Header() {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)

  return (
    <header className="fixed top-0 left-0 right-0 z-50 bg-white/95 backdrop-blur-sm border-b border-gray-200">
      <div className="container">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link href="/" className="flex items-center space-x-2">
            <div className="flex items-center justify-center w-8 h-8 bg-gradient-to-r from-blue-600 to-purple-600 rounded-lg">
              <Plane className="w-5 h-5 text-white" />
            </div>
            <span className="text-xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              EmptyJet
            </span>
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center space-x-8">
            <Link 
              href="/search" 
              className="text-gray-700 hover:text-blue-600 transition-colors"
            >
              Rechercher
            </Link>
            <Link 
              href="/yachts" 
              className="text-gray-700 hover:text-blue-600 transition-colors"
            >
              Yachts
            </Link>
            <Link 
              href="/cars" 
              className="text-gray-700 hover:text-blue-600 transition-colors"
            >
              Voitures
            </Link>
            <Link 
              href="/hotels" 
              className="text-gray-700 hover:text-blue-600 transition-colors"
            >
              Hôtels
            </Link>
            <Link 
              href="/about" 
              className="text-gray-700 hover:text-blue-600 transition-colors"
            >
              À propos
            </Link>
          </nav>

          {/* Desktop Auth Buttons */}
          <div className="hidden md:flex items-center space-x-4">
            <Link 
              href="/dashboard"
              className="flex items-center space-x-1 text-gray-700 hover:text-blue-600 transition-colors"
            >
              <User className="w-4 h-4" />
              <span>Mon compte</span>
            </Link>
            <Link 
              href="/company/login"
              className="text-gray-700 hover:text-blue-600 transition-colors"
            >
              Espace Compagnie
            </Link>
            <Link 
              href="/login"
              className="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
            >
              Connexion
            </Link>
            <Link 
              href="/register"
              className="px-4 py-2 text-sm font-medium text-white bg-gradient-to-r from-blue-600 to-purple-600 rounded-lg hover:from-blue-700 hover:to-purple-700 transition-colors"
            >
              Inscription
            </Link>
          </div>

          {/* Mobile menu button */}
          <button
            className="md:hidden p-2"
            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            aria-label="Toggle menu"
          >
            {isMobileMenuOpen ? (
              <X className="w-6 h-6" />
            ) : (
              <Menu className="w-6 h-6" />
            )}
          </button>
        </div>

        {/* Mobile Navigation */}
        {isMobileMenuOpen && (
          <div className="md:hidden">
            <div className="px-2 pt-2 pb-3 space-y-1 bg-white border-t border-gray-200">
              <Link 
                href="/search"
                className="block px-3 py-2 text-gray-700 hover:text-primary transition-colors"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                Rechercher
              </Link>
              <Link 
                href="/how-it-works"
                className="block px-3 py-2 text-gray-700 hover:text-primary transition-colors"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                Comment ça marche
              </Link>
              <Link 
                href="/about"
                className="block px-3 py-2 text-gray-700 hover:text-primary transition-colors"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                À propos
              </Link>
              <Link 
                href="/help"
                className="block px-3 py-2 text-gray-700 hover:text-primary transition-colors"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                Aide
              </Link>
              <div className="pt-4 pb-3 border-t border-gray-200">
                <Link 
                  href="/company/login"
                  className="block px-3 py-2 text-gray-700 hover:text-primary transition-colors"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Espace Compagnie
                </Link>
                <div className="flex space-x-2 px-3 pt-2">
                  <Link 
                    href="/login"
                    className="btn btn-outline btn-sm flex-1"
                    onClick={() => setIsMobileMenuOpen(false)}
                  >
                    Connexion
                  </Link>
                  <Link 
                    href="/register"
                    className="btn btn-primary btn-sm flex-1"
                    onClick={() => setIsMobileMenuOpen(false)}
                  >
                    Inscription
                  </Link>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </header>
  )
}