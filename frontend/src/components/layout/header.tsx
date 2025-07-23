'use client'

import { useState } from 'react'
import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { useAuthStore } from '@/store/auth.store'
import { UserRole } from '@/types'
import { 
  Plane, 
  User, 
  LogOut, 
  Settings, 
  Calendar,
  Building2,
  BarChart3,
  Menu,
  X
} from 'lucide-react'

export function Header() {
  const router = useRouter()
  const { user, isAuthenticated, logout } = useAuthStore()
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)

  const handleLogout = async () => {
    await logout()
    router.push('/')
  }

  const navigation = [
    { name: 'Search Flights', href: '/flights/search' },
    { name: 'How it Works', href: '/how-it-works' },
    { name: 'About', href: '/about' },
  ]

  const userNavigation = [
    { name: 'My Bookings', href: '/dashboard/bookings', icon: Calendar },
    { name: 'Profile', href: '/dashboard/profile', icon: User },
    { name: 'Settings', href: '/dashboard/settings', icon: Settings },
  ]

  const companyNavigation = [
    { name: 'Dashboard', href: '/company/dashboard', icon: BarChart3 },
    { name: 'Flights', href: '/company/flights', icon: Plane },
    { name: 'Aircraft', href: '/company/aircraft', icon: Building2 },
    { name: 'Bookings', href: '/company/bookings', icon: Calendar },
  ]

  const adminNavigation = [
    { name: 'Admin Dashboard', href: '/admin/dashboard', icon: BarChart3 },
    { name: 'Users', href: '/admin/users', icon: User },
    { name: 'Companies', href: '/admin/companies', icon: Building2 },
    { name: 'Flights', href: '/admin/flights', icon: Plane },
  ]

  const getRoleBasedNavigation = () => {
    if (!user) return []
    
    switch (user.role) {
      case UserRole.Company:
        return companyNavigation
      case UserRole.Admin:
        return adminNavigation
      default:
        return userNavigation
    }
  }

  return (
    <header className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container flex h-16 items-center">
        {/* Logo */}
        <Link href="/" className="flex items-center space-x-2">
          <Plane className="h-6 w-6 text-primary" />
          <span className="font-bold text-xl">Empty Legs</span>
        </Link>

        {/* Desktop Navigation */}
        <nav className="hidden md:flex items-center space-x-6 text-sm font-medium ml-6">
          {navigation.map((item) => (
            <Link
              key={item.name}
              href={item.href}
              className="transition-colors hover:text-foreground/80 text-foreground/60"
            >
              {item.name}
            </Link>
          ))}
        </nav>

        <div className="flex items-center justify-end flex-1">
          {/* Desktop User Menu */}
          {isAuthenticated ? (
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" className="relative h-8 w-8 rounded-full">
                  <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary text-primary-foreground">
                    {user?.firstName?.[0]?.toUpperCase() || 'U'}
                  </div>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent className="w-56" align="end" forceMount>
                <DropdownMenuLabel className="font-normal">
                  <div className="flex flex-col space-y-1">
                    <p className="text-sm font-medium leading-none">
                      {user?.firstName} {user?.lastName}
                    </p>
                    <p className="text-xs leading-none text-muted-foreground">
                      {user?.email}
                    </p>
                  </div>
                </DropdownMenuLabel>
                <DropdownMenuSeparator />
                {getRoleBasedNavigation().map((item) => (
                  <DropdownMenuItem key={item.name} asChild>
                    <Link href={item.href} className="flex items-center">
                      <item.icon className="mr-2 h-4 w-4" />
                      {item.name}
                    </Link>
                  </DropdownMenuItem>
                ))}
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={handleLogout}>
                  <LogOut className="mr-2 h-4 w-4" />
                  Log out
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          ) : (
            <div className="hidden md:flex items-center space-x-4">
              <Button variant="ghost" asChild>
                <Link href="/auth/login">Login</Link>
              </Button>
              <Button asChild>
                <Link href="/auth/register">Sign Up</Link>
              </Button>
            </div>
          )}

          {/* Mobile Menu Button */}
          <Button
            variant="ghost"
            className="md:hidden"
            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
          >
            {isMobileMenuOpen ? (
              <X className="h-6 w-6" />
            ) : (
              <Menu className="h-6 w-6" />
            )}
          </Button>
        </div>
      </div>

      {/* Mobile Navigation */}
      {isMobileMenuOpen && (
        <div className="md:hidden">
          <div className="space-y-1 px-4 pb-3 pt-2">
            {navigation.map((item) => (
              <Link
                key={item.name}
                href={item.href}
                className="block rounded-md px-3 py-2 text-base font-medium transition-colors hover:bg-accent"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                {item.name}
              </Link>
            ))}
            
            {isAuthenticated ? (
              <>
                <div className="border-t pt-4">
                  {getRoleBasedNavigation().map((item) => (
                    <Link
                      key={item.name}
                      href={item.href}
                      className="flex items-center rounded-md px-3 py-2 text-base font-medium transition-colors hover:bg-accent"
                      onClick={() => setIsMobileMenuOpen(false)}
                    >
                      <item.icon className="mr-2 h-4 w-4" />
                      {item.name}
                    </Link>
                  ))}
                  <button
                    onClick={() => {
                      handleLogout()
                      setIsMobileMenuOpen(false)
                    }}
                    className="flex w-full items-center rounded-md px-3 py-2 text-base font-medium transition-colors hover:bg-accent"
                  >
                    <LogOut className="mr-2 h-4 w-4" />
                    Log out
                  </button>
                </div>
              </>
            ) : (
              <div className="border-t pt-4 space-y-2">
                <Link
                  href="/auth/login"
                  className="block rounded-md px-3 py-2 text-base font-medium transition-colors hover:bg-accent"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Login
                </Link>
                <Link
                  href="/auth/register"
                  className="block rounded-md px-3 py-2 text-base font-medium transition-colors hover:bg-accent"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Sign Up
                </Link>
              </div>
            )}
          </div>
        </div>
      )}
    </header>
  )
}