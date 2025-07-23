'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { useAuthStore } from '@/store/auth.store'
import { UserRole } from '@/types'
import { Header } from '@/components/layout/header'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { 
  Calendar, 
  Plane, 
  User, 
  BarChart3,
  ArrowRight,
  Clock,
  CheckCircle
} from 'lucide-react'
import Link from 'next/link'

export default function DashboardPage() {
  const router = useRouter()
  const { user, isAuthenticated } = useAuthStore()

  useEffect(() => {
    if (!isAuthenticated) {
      router.push('/auth/login')
      return
    }

    // Redirect based on user role
    if (user?.role === UserRole.Company) {
      router.push('/company/dashboard')
    } else if (user?.role === UserRole.Admin) {
      router.push('/admin/dashboard')
    }
  }, [isAuthenticated, user, router])

  if (!isAuthenticated || !user) {
    return (
      <div className="flex flex-col min-h-screen">
        <Header />
        <main className="flex-1 flex items-center justify-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
        </main>
      </div>
    )
  }

  // Customer dashboard
  return (
    <div className="flex flex-col min-h-screen">
      <Header />
      
      <main className="flex-1 container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold mb-2">
            Welcome back, {user.firstName}!
          </h1>
          <p className="text-muted-foreground">
            Ready to find your next empty leg flight?
          </p>
        </div>

        {/* Quick Actions */}
        <div className="grid md:grid-cols-3 gap-6 mb-8">
          <Card className="hover:shadow-lg transition-shadow">
            <CardContent className="p-6">
              <div className="flex items-center mb-4">
                <div className="p-2 bg-primary/10 rounded-lg">
                  <Plane className="w-6 h-6 text-primary" />
                </div>
                <h3 className="font-semibold ml-3">Search Flights</h3>
              </div>
              <p className="text-sm text-muted-foreground mb-4">
                Find the perfect empty leg flight for your next trip
              </p>
              <Button asChild className="w-full">
                <Link href="/flights/search">
                  Search Now
                  <ArrowRight className="w-4 h-4 ml-2" />
                </Link>
              </Button>
            </CardContent>
          </Card>

          <Card className="hover:shadow-lg transition-shadow">
            <CardContent className="p-6">
              <div className="flex items-center mb-4">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <Calendar className="w-6 h-6 text-blue-600" />
                </div>
                <h3 className="font-semibold ml-3">My Bookings</h3>
              </div>
              <p className="text-sm text-muted-foreground mb-4">
                View and manage your flight reservations
              </p>
              <Button variant="outline" asChild className="w-full">
                <Link href="/dashboard/bookings">
                  View Bookings
                  <ArrowRight className="w-4 h-4 ml-2" />
                </Link>
              </Button>
            </CardContent>
          </Card>

          <Card className="hover:shadow-lg transition-shadow">
            <CardContent className="p-6">
              <div className="flex items-center mb-4">
                <div className="p-2 bg-green-100 rounded-lg">
                  <User className="w-6 h-6 text-green-600" />
                </div>
                <h3 className="font-semibold ml-3">My Profile</h3>
              </div>
              <p className="text-sm text-muted-foreground mb-4">
                Update your personal information and preferences
              </p>
              <Button variant="outline" asChild className="w-full">
                <Link href="/dashboard/profile">
                  Edit Profile
                  <ArrowRight className="w-4 h-4 ml-2" />
                </Link>
              </Button>
            </CardContent>
          </Card>
        </div>

        {/* Recent Activity */}
        <div className="grid lg:grid-cols-2 gap-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center">
                <Clock className="w-5 h-5 mr-2" />
                Recent Activity
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="text-center py-8 text-muted-foreground">
                  <Calendar className="w-12 h-12 mx-auto mb-4 opacity-50" />
                  <p>No recent activity</p>
                  <p className="text-sm">Your bookings and searches will appear here</p>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="flex items-center">
                <CheckCircle className="w-5 h-5 mr-2" />
                Account Status
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <span>Email Verification</span>
                  {user.isEmailVerified ? (
                    <div className="flex items-center text-green-600">
                      <CheckCircle className="w-4 h-4 mr-1" />
                      Verified
                    </div>
                  ) : (
                    <div className="flex items-center text-yellow-600">
                      <Clock className="w-4 h-4 mr-1" />
                      Pending
                    </div>
                  )}
                </div>

                <div className="flex items-center justify-between">
                  <span>Account Status</span>
                  {user.isActive ? (
                    <div className="flex items-center text-green-600">
                      <CheckCircle className="w-4 h-4 mr-1" />
                      Active
                    </div>
                  ) : (
                    <div className="flex items-center text-red-600">
                      <Clock className="w-4 h-4 mr-1" />
                      Inactive
                    </div>
                  )}
                </div>

                <div className="flex items-center justify-between">
                  <span>Member Since</span>
                  <span className="text-sm text-muted-foreground">
                    {new Date(user.createdAt).toLocaleDateString()}
                  </span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  )
}