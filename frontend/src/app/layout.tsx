import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import './globals.css'
import { QueryProvider } from '../components/providers/QueryProvider'
import { ToastProvider } from '../components/providers/ToastProvider'

const inter = Inter({ subsets: ['latin'] })

export const metadata: Metadata = {
  title: 'Empty Legs - Aviation Privée à Prix Réduits',
  description: 'Trouvez et réservez des vols à vide en jet privé à prix avantageux. Plateforme de mise en relation entre compagnies aériennes et passagers.',
  keywords: ['jet privé', 'empty legs', 'vols à vide', 'aviation privée', 'vol pas cher'],
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="fr">
      <body className={inter.className}>
        <QueryProvider>
          {children}
          <ToastProvider />
        </QueryProvider>
      </body>
    </html>
  )
}