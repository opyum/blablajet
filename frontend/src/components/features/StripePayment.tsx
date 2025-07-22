'use client'

import { useState, useEffect } from 'react'
import { loadStripe } from '@stripe/stripe-js'
import { Elements, CardElement, useStripe, useElements } from '@stripe/react-stripe-js'
import { CreditCard, Lock, Shield, Check, Euro } from 'lucide-react'

const stripePromise = loadStripe(process.env.NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY || '')

interface BookingDetails {
  id: string
  type: 'flight' | 'yacht' | 'car' | 'hotel'
  title: string
  amount: number
  currency: string
  description: string
  image: string
}

interface PaymentFormProps {
  booking: BookingDetails
  onSuccess: (paymentId: string) => void
  onError: (error: string) => void
}

function PaymentForm({ booking, onSuccess, onError }: PaymentFormProps) {
  const stripe = useStripe()
  const elements = useElements()
  const [processing, setProcessing] = useState(false)
  const [clientSecret, setClientSecret] = useState('')
  const [paymentMethod, setPaymentMethod] = useState<'card' | 'apple_pay' | 'google_pay'>('card')
  const [formData, setFormData] = useState({
    email: '',
    name: '',
    address: {
      line1: '',
      city: '',
      postal_code: '',
      country: 'FR'
    }
  })

  useEffect(() => {
    // Create payment intent
    const createPaymentIntent = async () => {
      try {
        const response = await fetch('/api/payments/create-intent', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            amount: booking.amount * 100, // Convert to cents
            currency: booking.currency.toLowerCase(),
            bookingId: booking.id,
            description: booking.description
          }),
        })

        const data = await response.json()
        
        if (data.clientSecret) {
          setClientSecret(data.clientSecret)
        } else {
          onError('Erreur lors de l\'initialisation du paiement')
        }
      } catch (error) {
        onError('Erreur de connexion au serveur de paiement')
      }
    }

    createPaymentIntent()
  }, [booking, onError])

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()

    if (!stripe || !elements || !clientSecret) {
      return
    }

    setProcessing(true)

    const cardElement = elements.getElement(CardElement)

    if (!cardElement) {
      onError('Élément de carte non trouvé')
      setProcessing(false)
      return
    }

    try {
      const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
        payment_method: {
          card: cardElement,
          billing_details: {
            name: formData.name,
            email: formData.email,
            address: formData.address
          },
        },
      })

      if (error) {
        onError(error.message || 'Erreur lors du paiement')
      } else if (paymentIntent.status === 'succeeded') {
        onSuccess(paymentIntent.id)
      }
    } catch (err) {
      onError('Erreur inattendue lors du paiement')
    }

    setProcessing(false)
  }

  const cardElementOptions = {
    style: {
      base: {
        fontSize: '16px',
        color: '#424770',
        '::placeholder': {
          color: '#aab7c4',
        },
        fontFamily: 'system-ui, -apple-system, sans-serif',
      },
      invalid: {
        color: '#9e2146',
      },
    },
    hidePostalCode: true,
  }

  return (
    <div className="max-w-md mx-auto bg-white rounded-2xl shadow-2xl overflow-hidden">
      {/* Header */}
      <div className="bg-gradient-to-r from-blue-600 to-purple-600 p-6 text-white">
        <div className="flex items-center gap-3 mb-4">
          <div className="bg-white/20 rounded-full p-2">
            <Lock className="w-5 h-5" />
          </div>
          <h2 className="text-xl font-semibold">Paiement sécurisé</h2>
        </div>
        <div className="flex items-center gap-2 text-sm">
          <Shield className="w-4 h-4" />
          <span>Chiffrement SSL 256-bit</span>
        </div>
      </div>

      {/* Booking Summary */}
      <div className="p-6 border-b border-gray-200">
        <div className="flex items-start gap-4">
          <img 
            src={booking.image} 
            alt={booking.title}
            className="w-16 h-16 rounded-lg object-cover"
          />
          <div className="flex-1">
            <h3 className="font-semibold text-gray-900 mb-1">{booking.title}</h3>
            <p className="text-sm text-gray-600 mb-2">{booking.description}</p>
            <div className="flex items-center justify-between">
              <span className="text-sm text-gray-500">Total à payer</span>
              <div className="flex items-center gap-1 text-xl font-bold text-gray-900">
                <Euro className="w-5 h-5" />
                {booking.amount.toLocaleString()}
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Payment Form */}
      <form onSubmit={handleSubmit} className="p-6 space-y-6">
        {/* Contact Information */}
        <div className="space-y-4">
          <h4 className="font-medium text-gray-900">Informations de contact</h4>
          
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <input
              type="email"
              required
              value={formData.email}
              onChange={(e) => setFormData(prev => ({ ...prev, email: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              placeholder="votre@email.com"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Nom complet
            </label>
            <input
              type="text"
              required
              value={formData.name}
              onChange={(e) => setFormData(prev => ({ ...prev, name: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              placeholder="Jean Dupont"
            />
          </div>
        </div>

        {/* Payment Method Selection */}
        <div className="space-y-4">
          <h4 className="font-medium text-gray-900">Mode de paiement</h4>
          
          <div className="space-y-2">
            <label className="flex items-center p-3 border border-gray-200 rounded-lg cursor-pointer hover:bg-gray-50">
              <input
                type="radio"
                name="paymentMethod"
                value="card"
                checked={paymentMethod === 'card'}
                onChange={(e) => setPaymentMethod(e.target.value as any)}
                className="mr-3"
              />
              <CreditCard className="w-5 h-5 mr-2 text-gray-600" />
              <span className="font-medium">Carte bancaire</span>
            </label>
            
            {/* Payment methods can be extended here */}
          </div>
        </div>

        {/* Card Details */}
        {paymentMethod === 'card' && (
          <div className="space-y-4">
            <h4 className="font-medium text-gray-900">Détails de la carte</h4>
            
            <div className="p-3 border border-gray-300 rounded-lg">
              <CardElement options={cardElementOptions} />
            </div>

            {/* Billing Address */}
            <div className="space-y-3">
              <h5 className="text-sm font-medium text-gray-700">Adresse de facturation</h5>
              
              <input
                type="text"
                required
                value={formData.address.line1}
                onChange={(e) => setFormData(prev => ({ 
                  ...prev, 
                  address: { ...prev.address, line1: e.target.value }
                }))}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Adresse"
              />
              
              <div className="grid grid-cols-2 gap-3">
                <input
                  type="text"
                  required
                  value={formData.address.city}
                  onChange={(e) => setFormData(prev => ({ 
                    ...prev, 
                    address: { ...prev.address, city: e.target.value }
                  }))}
                  className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Ville"
                />
                
                <input
                  type="text"
                  required
                  value={formData.address.postal_code}
                  onChange={(e) => setFormData(prev => ({ 
                    ...prev, 
                    address: { ...prev.address, postal_code: e.target.value }
                  }))}
                  className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Code postal"
                />
              </div>
            </div>
          </div>
        )}

        {/* Security Notice */}
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <div className="flex items-start gap-3">
            <Shield className="w-5 h-5 text-blue-600 mt-0.5" />
            <div className="text-sm">
              <p className="font-medium text-blue-900 mb-1">Paiement sécurisé</p>
              <p className="text-blue-700">
                Vos informations sont protégées par un chiffrement SSL et ne sont jamais stockées sur nos serveurs.
              </p>
            </div>
          </div>
        </div>

        {/* Submit Button */}
        <button
          type="submit"
          disabled={!stripe || processing || !clientSecret}
          className="w-full bg-gradient-to-r from-blue-600 to-purple-600 text-white py-3 px-4 rounded-lg font-semibold hover:from-blue-700 hover:to-purple-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 flex items-center justify-center gap-2"
        >
          {processing ? (
            <>
              <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
              Traitement en cours...
            </>
          ) : (
            <>
              <Lock className="w-5 h-5" />
              Payer {booking.amount.toLocaleString()} €
            </>
          )}
        </button>

        {/* Trust Badges */}
        <div className="flex items-center justify-center gap-4 pt-4 border-t border-gray-200">
          <div className="flex items-center gap-2 text-xs text-gray-500">
            <Check className="w-4 h-4 text-green-500" />
            <span>Stripe</span>
          </div>
          <div className="flex items-center gap-2 text-xs text-gray-500">
            <Check className="w-4 h-4 text-green-500" />
            <span>SSL Sécurisé</span>
          </div>
          <div className="flex items-center gap-2 text-xs text-gray-500">
            <Check className="w-4 h-4 text-green-500" />
            <span>RGPD</span>
          </div>
        </div>
      </form>
    </div>
  )
}

interface StripePaymentProps {
  booking: BookingDetails
  onSuccess: (paymentId: string) => void
  onError: (error: string) => void
}

export function StripePayment({ booking, onSuccess, onError }: StripePaymentProps) {
  return (
    <Elements stripe={stripePromise}>
      <PaymentForm 
        booking={booking}
        onSuccess={onSuccess}
        onError={onError}
      />
    </Elements>
  )
}