'use client'

import { SearchFlightsForm } from '@/components/forms/search-flights-form'
import { Header } from '@/components/layout/header'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { 
  Plane, 
  Clock, 
  DollarSign, 
  Shield, 
  Star,
  ArrowRight,
  CheckCircle
} from 'lucide-react'
import Link from 'next/link'

export default function HomePage() {
  const features = [
    {
      icon: DollarSign,
      title: 'Discounted Prices',
      description: 'Save up to 75% on private jet flights with empty leg deals',
    },
    {
      icon: Clock,
      title: 'Last-Minute Availability',
      description: 'Find immediate departure flights and book within hours',
    },
    {
      icon: Shield,
      title: 'Verified Operators',
      description: 'All aircraft operators are verified and certified',
    },
    {
      icon: Star,
      title: 'Premium Experience',
      description: 'Enjoy luxury private jet travel at accessible prices',
    },
  ]

  const howItWorks = [
    {
      step: '1',
      title: 'Search',
      description: 'Enter your departure and arrival cities with travel dates',
    },
    {
      step: '2',
      title: 'Compare',
      description: 'Browse available empty leg flights and compare prices',
    },
    {
      step: '3',
      title: 'Book',
      description: 'Secure your flight with instant booking confirmation',
    },
    {
      step: '4',
      title: 'Fly',
      description: 'Enjoy your premium private jet experience',
    },
  ]

  const testimonials = [
    {
      name: 'Sarah Johnson',
      role: 'Business Executive',
      content: 'Found an amazing deal from NYC to Miami. The whole process was seamless and professional.',
      rating: 5,
    },
    {
      name: 'Michael Chen',
      role: 'Entrepreneur',
      content: 'Empty Legs made private jet travel accessible for my business trips. Highly recommend!',
      rating: 5,
    },
    {
      name: 'Emma Rodriguez',
      role: 'Frequent Traveler',
      content: 'The time saved and comfort provided makes this service invaluable for urgent travel.',
      rating: 5,
    },
  ]

  return (
    <div className="flex flex-col min-h-screen">
      <Header />
      
      <main className="flex-1">
        {/* Hero Section */}
        <section className="relative bg-gradient-to-br from-blue-50 to-indigo-100 py-20 lg:py-32">
          <div className="container mx-auto px-4">
            <div className="text-center max-w-4xl mx-auto mb-12">
              <h1 className="text-4xl md:text-6xl font-bold tracking-tight mb-6">
                Private Jet Travel
                <span className="text-primary"> Made Accessible</span>
              </h1>
              <p className="text-xl md:text-2xl text-muted-foreground mb-8">
                Discover empty leg flights at up to 75% off regular private jet prices. 
                Book luxury travel on your schedule.
              </p>
            </div>

            {/* Search Form */}
            <div className="max-w-4xl mx-auto">
              <Card className="shadow-xl">
                <CardHeader>
                  <CardTitle className="text-center">Find Your Perfect Flight</CardTitle>
                  <CardDescription className="text-center">
                    Search thousands of empty leg flights worldwide
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <SearchFlightsForm />
                </CardContent>
              </Card>
            </div>
          </div>
        </section>

        {/* Features Section */}
        <section className="py-20">
          <div className="container mx-auto px-4">
            <div className="text-center mb-16">
              <h2 className="text-3xl md:text-4xl font-bold mb-4">
                Why Choose Empty Legs?
              </h2>
              <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
                Experience the benefits of private jet travel without the premium price tag
              </p>
            </div>

            <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
              {features.map((feature, index) => (
                <Card key={index} className="text-center hover:shadow-lg transition-shadow">
                  <CardContent className="pt-6">
                    <div className="w-12 h-12 mx-auto mb-4 rounded-full bg-primary/10 flex items-center justify-center">
                      <feature.icon className="w-6 h-6 text-primary" />
                    </div>
                    <h3 className="font-semibold mb-2">{feature.title}</h3>
                    <p className="text-sm text-muted-foreground">{feature.description}</p>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>
        </section>

        {/* How It Works Section */}
        <section className="py-20 bg-muted/30">
          <div className="container mx-auto px-4">
            <div className="text-center mb-16">
              <h2 className="text-3xl md:text-4xl font-bold mb-4">
                How It Works
              </h2>
              <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
                Book your empty leg flight in four simple steps
              </p>
            </div>

            <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
              {howItWorks.map((step, index) => (
                <div key={index} className="text-center">
                  <div className="relative mb-6">
                    <div className="w-16 h-16 mx-auto rounded-full bg-primary text-primary-foreground flex items-center justify-center text-2xl font-bold">
                      {step.step}
                    </div>
                    {index < howItWorks.length - 1 && (
                      <ArrowRight className="hidden lg:block absolute top-1/2 -translate-y-1/2 left-full w-8 h-8 text-muted-foreground" />
                    )}
                  </div>
                  <h3 className="font-semibold mb-2">{step.title}</h3>
                  <p className="text-sm text-muted-foreground">{step.description}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        {/* Testimonials Section */}
        <section className="py-20">
          <div className="container mx-auto px-4">
            <div className="text-center mb-16">
              <h2 className="text-3xl md:text-4xl font-bold mb-4">
                What Our Customers Say
              </h2>
              <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
                Join thousands of satisfied travelers who have discovered empty leg flights
              </p>
            </div>

            <div className="grid md:grid-cols-3 gap-8">
              {testimonials.map((testimonial, index) => (
                <Card key={index} className="hover:shadow-lg transition-shadow">
                  <CardContent className="pt-6">
                    <div className="flex items-center mb-4">
                      {[...Array(testimonial.rating)].map((_, i) => (
                        <Star key={i} className="w-5 h-5 fill-yellow-400 text-yellow-400" />
                      ))}
                    </div>
                    <p className="text-muted-foreground mb-4">"{testimonial.content}"</p>
                    <div>
                      <p className="font-semibold">{testimonial.name}</p>
                      <p className="text-sm text-muted-foreground">{testimonial.role}</p>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>
        </section>

        {/* CTA Section */}
        <section className="py-20 bg-primary text-primary-foreground">
          <div className="container mx-auto px-4 text-center">
            <h2 className="text-3xl md:text-4xl font-bold mb-4">
              Ready to Fly?
            </h2>
            <p className="text-xl mb-8 opacity-90">
              Start searching for empty leg flights and experience luxury travel today
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button size="lg" variant="secondary" asChild>
                <Link href="/flights/search">
                  Search Flights
                  <Plane className="ml-2 w-5 h-5" />
                </Link>
              </Button>
              <Button size="lg" variant="outline" asChild>
                <Link href="/auth/register">
                  Sign Up Free
                </Link>
              </Button>
            </div>
          </div>
        </section>
      </main>

      {/* Footer */}
      <footer className="border-t py-12">
        <div className="container mx-auto px-4">
          <div className="grid md:grid-cols-4 gap-8">
            <div>
              <div className="flex items-center space-x-2 mb-4">
                <Plane className="h-6 w-6 text-primary" />
                <span className="font-bold text-lg">Empty Legs</span>
              </div>
              <p className="text-muted-foreground">
                Making private jet travel accessible through empty leg flights.
              </p>
            </div>
            
            <div>
              <h3 className="font-semibold mb-4">Company</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li><Link href="/about" className="hover:text-foreground">About Us</Link></li>
                <li><Link href="/how-it-works" className="hover:text-foreground">How It Works</Link></li>
                <li><Link href="/contact" className="hover:text-foreground">Contact</Link></li>
              </ul>
            </div>
            
            <div>
              <h3 className="font-semibold mb-4">Support</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li><Link href="/help" className="hover:text-foreground">Help Center</Link></li>
                <li><Link href="/safety" className="hover:text-foreground">Safety</Link></li>
                <li><Link href="/terms" className="hover:text-foreground">Terms of Service</Link></li>
                <li><Link href="/privacy" className="hover:text-foreground">Privacy Policy</Link></li>
              </ul>
            </div>
            
            <div>
              <h3 className="font-semibold mb-4">Operators</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li><Link href="/operators/join" className="hover:text-foreground">Join as Operator</Link></li>
                <li><Link href="/operators/resources" className="hover:text-foreground">Resources</Link></li>
                <li><Link href="/operators/support" className="hover:text-foreground">Operator Support</Link></li>
              </ul>
            </div>
          </div>
          
          <div className="border-t mt-8 pt-8 text-center text-sm text-muted-foreground">
            <p>&copy; 2024 Empty Legs. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  )
}