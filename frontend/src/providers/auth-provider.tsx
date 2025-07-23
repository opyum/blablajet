'use client'

import { useEffect } from 'react'
import { useAuthStore, initializeAuth } from '@/store/auth.store'

export function AuthProvider({ children }: { children: React.ReactNode }) {
  useEffect(() => {
    initializeAuth()
  }, [])

  return <>{children}</>
}