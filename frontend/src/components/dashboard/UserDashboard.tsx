'use client'

import { useState, useEffect } from 'react'
import { User, Calendar, Heart, MapPin, Clock, Star, Euro, Plane, Anchor, Car, Building, CreditCard, Settings, Bell, LogOut } from 'lucide-react'

interface UserProfile {
  id: string
  email: string
  firstName: string
  lastName: string
  phoneNumber?: string
  avatar?: string
  memberSince: string
  totalBookings: number
  totalSpent: number
  loyaltyLevel: string
}

interface BookingItem {
  id: string
  type: 'flight' | 'yacht' | 'car' | 'hotel'
  title: string
  image: string
  bookingDate: string
  travelDate: string
  status: 'confirmed' | 'pending' | 'completed' | 'cancelled'
  amount: number
  reference: string
  location: string
  duration?: string
}

interface WishlistItem {
  id: string
  type: 'flight' | 'yacht' | 'car' | 'hotel'
  title: string
  image: string
  price: number
  location: string
  addedDate: string
  luxuryLevel: string
}

export function UserDashboard() {
  const [activeTab, setActiveTab] = useState<'overview' | 'bookings' | 'wishlist' | 'profile'>('overview')
  const [user, setUser] = useState<UserProfile | null>(null)
  const [bookings, setBookings] = useState<BookingItem[]>([])
  const [wishlist, setWishlist] = useState<WishlistItem[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    // Simulate API calls
    setTimeout(() => {
      setUser({
        id: '1',
        email: 'user@example.com',
        firstName: 'Pierre',
        lastName: 'Dubois',
        phoneNumber: '+33 1 23 45 67 89',
        avatar: '/api/placeholder/100/100',
        memberSince: '2023-01-15',
        totalBookings: 8,
        totalSpent: 45000,
        loyaltyLevel: 'Gold'
      })

      setBookings([
        {
          id: '1',
          type: 'flight',
          title: 'Citation CJ3+ - Paris → Nice',
          image: '/api/placeholder/300/200',
          bookingDate: '2024-01-15',
          travelDate: '2024-02-10',
          status: 'confirmed',
          amount: 8500,
          reference: 'EL12345678',
          location: 'Paris, France',
          duration: '1h 30min'
        },
        {
          id: '2',
          type: 'yacht',
          title: 'Sunseeker 86 - Côte d\'Azur',
          image: '/api/placeholder/300/200',
          bookingDate: '2024-01-10',
          travelDate: '2024-03-15',
          status: 'pending',
          amount: 12000,
          reference: 'YT87654321',
          location: 'Monaco',
          duration: '3 jours'
        }
      ])

      setWishlist([
        {
          id: '1',
          type: 'car',
          title: 'Ferrari 488 Spider',
          image: '/api/placeholder/300/200',
          price: 1200,
          location: 'Monaco',
          addedDate: '2024-01-20',
          luxuryLevel: 'exclusive'
        },
        {
          id: '2',
          type: 'hotel',
          title: 'The Ritz Paris - Suite Présidentielle',
          image: '/api/placeholder/300/200',
          price: 3500,
          location: 'Paris, France',
          addedDate: '2024-01-18',
          luxuryLevel: 'ultra-luxury'
        }
      ])

      setLoading(false)
    }, 1000)
  }, [])

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'confirmed': return 'bg-green-100 text-green-800'
      case 'pending': return 'bg-yellow-100 text-yellow-800'
      case 'completed': return 'bg-blue-100 text-blue-800'
      case 'cancelled': return 'bg-red-100 text-red-800'
      default: return 'bg-gray-100 text-gray-800'
    }
  }

  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'confirmed': return 'Confirmé'
      case 'pending': return 'En attente'
      case 'completed': return 'Terminé'
      case 'cancelled': return 'Annulé'
      default: return status
    }
  }

  const getVehicleIcon = (type: string) => {
    switch (type) {
      case 'flight': return Plane
      case 'yacht': return Anchor
      case 'car': return Car
      case 'hotel': return Building
      default: return MapPin
    }
  }

  const getLoyaltyLevelColor = (level: string) => {
    switch (level) {
      case 'Bronze': return 'text-orange-600'
      case 'Silver': return 'text-gray-500'
      case 'Gold': return 'text-yellow-500'
      case 'Platinum': return 'text-purple-600'
      default: return 'text-gray-600'
    }
  }

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-6">
            <div className="flex items-center gap-4">
              <img
                src={user?.avatar}
                alt={`${user?.firstName} ${user?.lastName}`}
                className="w-12 h-12 rounded-full object-cover"
              />
              <div>
                <h1 className="text-2xl font-bold text-gray-900">
                  Bonjour, {user?.firstName} !
                </h1>
                <p className="text-gray-600">
                  Membre {user?.loyaltyLevel} depuis {new Date(user?.memberSince || '').toLocaleDateString('fr-FR')}
                </p>
              </div>
            </div>
            <div className="flex items-center gap-4">
              <button className="p-2 text-gray-600 hover:text-gray-900">
                <Bell className="w-6 h-6" />
              </button>
              <button className="p-2 text-gray-600 hover:text-gray-900">
                <Settings className="w-6 h-6" />
              </button>
              <button className="p-2 text-gray-600 hover:text-gray-900">
                <LogOut className="w-6 h-6" />
              </button>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Navigation Tabs */}
        <div className="bg-white rounded-lg shadow-sm mb-8">
          <div className="border-b border-gray-200">
            <nav className="flex space-x-8 px-6">
              {[
                { key: 'overview', label: 'Vue d\'ensemble', icon: User },
                { key: 'bookings', label: 'Mes réservations', icon: Calendar },
                { key: 'wishlist', label: 'Ma wishlist', icon: Heart },
                { key: 'profile', label: 'Mon profil', icon: Settings }
              ].map(tab => {
                const Icon = tab.icon
                return (
                  <button
                    key={tab.key}
                    onClick={() => setActiveTab(tab.key as any)}
                    className={`flex items-center gap-2 py-4 px-1 border-b-2 font-medium text-sm ${
                      activeTab === tab.key
                        ? 'border-blue-500 text-blue-600'
                        : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                    }`}
                  >
                    <Icon className="w-5 h-5" />
                    {tab.label}
                  </button>
                )
              })}
            </nav>
          </div>
        </div>

        {/* Content */}
        {activeTab === 'overview' && (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Stats Cards */}
            <div className="lg:col-span-2 grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="bg-white rounded-lg shadow-sm p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Total dépensé</p>
                    <p className="text-3xl font-bold text-gray-900">
                      {user?.totalSpent?.toLocaleString()} €
                    </p>
                  </div>
                  <div className="bg-blue-100 rounded-full p-3">
                    <Euro className="w-8 h-8 text-blue-600" />
                  </div>
                </div>
              </div>

              <div className="bg-white rounded-lg shadow-sm p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Réservations</p>
                    <p className="text-3xl font-bold text-gray-900">{user?.totalBookings}</p>
                  </div>
                  <div className="bg-green-100 rounded-full p-3">
                    <Calendar className="w-8 h-8 text-green-600" />
                  </div>
                </div>
              </div>

              <div className="bg-white rounded-lg shadow-sm p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Niveau de fidélité</p>
                    <p className={`text-3xl font-bold ${getLoyaltyLevelColor(user?.loyaltyLevel || '')}`}>
                      {user?.loyaltyLevel}
                    </p>
                  </div>
                  <div className="bg-yellow-100 rounded-full p-3">
                    <Star className="w-8 h-8 text-yellow-600" />
                  </div>
                </div>
              </div>

              <div className="bg-white rounded-lg shadow-sm p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Wishlist</p>
                    <p className="text-3xl font-bold text-gray-900">{wishlist.length}</p>
                  </div>
                  <div className="bg-pink-100 rounded-full p-3">
                    <Heart className="w-8 h-8 text-pink-600" />
                  </div>
                </div>
              </div>
            </div>

            {/* Recent Activity */}
            <div className="bg-white rounded-lg shadow-sm p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Activité récente</h3>
              <div className="space-y-4">
                {bookings.slice(0, 3).map(booking => {
                  const Icon = getVehicleIcon(booking.type)
                  return (
                    <div key={booking.id} className="flex items-start gap-3">
                      <div className="bg-blue-100 rounded-full p-2 mt-0.5">
                        <Icon className="w-4 h-4 text-blue-600" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-gray-900 truncate">
                          {booking.title}
                        </p>
                        <p className="text-sm text-gray-500">
                          {new Date(booking.bookingDate).toLocaleDateString('fr-FR')}
                        </p>
                      </div>
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(booking.status)}`}>
                        {getStatusLabel(booking.status)}
                      </span>
                    </div>
                  )
                })}
              </div>
            </div>
          </div>
        )}

        {activeTab === 'bookings' && (
          <div className="bg-white rounded-lg shadow-sm">
            <div className="px-6 py-4 border-b border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900">Mes réservations</h3>
            </div>
            <div className="divide-y divide-gray-200">
              {bookings.map(booking => {
                const Icon = getVehicleIcon(booking.type)
                return (
                  <div key={booking.id} className="p-6">
                    <div className="flex items-start gap-4">
                      <img
                        src={booking.image}
                        alt={booking.title}
                        className="w-24 h-24 rounded-lg object-cover"
                      />
                      <div className="flex-1">
                        <div className="flex items-start justify-between">
                          <div>
                            <div className="flex items-center gap-2 mb-1">
                              <Icon className="w-4 h-4 text-gray-600" />
                              <h4 className="text-lg font-semibold text-gray-900">
                                {booking.title}
                              </h4>
                            </div>
                            <p className="text-gray-600 mb-2">{booking.location}</p>
                            <div className="flex items-center gap-4 text-sm text-gray-500">
                              <span className="flex items-center gap-1">
                                <Calendar className="w-4 h-4" />
                                {new Date(booking.travelDate).toLocaleDateString('fr-FR')}
                              </span>
                              {booking.duration && (
                                <span className="flex items-center gap-1">
                                  <Clock className="w-4 h-4" />
                                  {booking.duration}
                                </span>
                              )}
                            </div>
                          </div>
                          <div className="text-right">
                            <p className="text-xl font-bold text-gray-900 mb-2">
                              {booking.amount.toLocaleString()} €
                            </p>
                            <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(booking.status)}`}>
                              {getStatusLabel(booking.status)}
                            </span>
                          </div>
                        </div>
                        <div className="mt-4 flex justify-between items-center">
                          <span className="text-sm text-gray-500">
                            Réf: {booking.reference}
                          </span>
                          <div className="flex gap-2">
                            <button className="px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100">
                              Voir détails
                            </button>
                            {booking.status === 'confirmed' && (
                              <button className="px-4 py-2 text-sm font-medium text-red-600 bg-red-50 rounded-lg hover:bg-red-100">
                                Annuler
                              </button>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                )
              })}
            </div>
          </div>
        )}

        {activeTab === 'wishlist' && (
          <div className="bg-white rounded-lg shadow-sm">
            <div className="px-6 py-4 border-b border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900">Ma wishlist</h3>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-6">
              {wishlist.map(item => {
                const Icon = getVehicleIcon(item.type)
                return (
                  <div key={item.id} className="border border-gray-200 rounded-lg overflow-hidden hover:shadow-lg transition-shadow">
                    <div className="relative">
                      <img
                        src={item.image}
                        alt={item.title}
                        className="w-full h-48 object-cover"
                      />
                      <button className="absolute top-2 right-2 p-2 bg-white rounded-full shadow-sm hover:bg-gray-50">
                        <Heart className="w-5 h-5 text-red-500 fill-current" />
                      </button>
                      <div className="absolute top-2 left-2 bg-white px-2 py-1 rounded-full text-xs font-medium flex items-center gap-1">
                        <Icon className="w-3 h-3" />
                        {item.type}
                      </div>
                    </div>
                    <div className="p-4">
                      <h4 className="font-semibold text-gray-900 mb-1">{item.title}</h4>
                      <p className="text-sm text-gray-600 mb-3">{item.location}</p>
                      <div className="flex items-center justify-between">
                        <span className="text-lg font-bold text-gray-900">
                          {item.price.toLocaleString()} €
                        </span>
                        <button className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700">
                          Réserver
                        </button>
                      </div>
                    </div>
                  </div>
                )
              })}
            </div>
          </div>
        )}

        {activeTab === 'profile' && (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <div className="bg-white rounded-lg shadow-sm p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-6">Informations personnelles</h3>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Prénom</label>
                  <input
                    type="text"
                    value={user?.firstName}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Nom</label>
                  <input
                    type="text"
                    value={user?.lastName}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                  <input
                    type="email"
                    value={user?.email}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Téléphone</label>
                  <input
                    type="tel"
                    value={user?.phoneNumber}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>
                <button className="w-full bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors">
                  Mettre à jour
                </button>
              </div>
            </div>

            <div className="bg-white rounded-lg shadow-sm p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-6">Préférences</h3>
              <div className="space-y-4">
                <div>
                  <label className="flex items-center">
                    <input type="checkbox" className="mr-3" />
                    <span className="text-sm text-gray-700">Recevoir les notifications par email</span>
                  </label>
                </div>
                <div>
                  <label className="flex items-center">
                    <input type="checkbox" className="mr-3" />
                    <span className="text-sm text-gray-700">Recevoir les offres spéciales</span>
                  </label>
                </div>
                <div>
                  <label className="flex items-center">
                    <input type="checkbox" className="mr-3" />
                    <span className="text-sm text-gray-700">Notifications push</span>
                  </label>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}