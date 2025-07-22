'use client'

import * as Toast from '@radix-ui/react-toast'
import { useState } from 'react'

export function ToastProvider() {
  const [open, setOpen] = useState(false)

  return (
    <Toast.Provider swipeDirection="right">
      <Toast.Viewport className="fixed bottom-0 right-0 flex flex-col p-6 gap-2 w-390 max-w-[100vw] m-0 list-none z-50 outline-none" />
    </Toast.Provider>
  )
}