import { AuthSessionData } from '@/lib/auth'
import { useEffect, useState } from 'react'

export default function useSession() {
  const [session, setSession] = useState<AuthSessionData | null>(null)
  const [loading, setLoading] = useState(true)
  useEffect(() => {
    const fetchSession = async () => {
      try {
        const response = await fetch('/api/session')
        if (response.ok) {
          const session = (await response.json()) as AuthSessionData
          setSession(session)
        }
      } finally {
        setLoading(false)
      }
    }
    fetchSession()
  }, [])
  return { session, loading }
}
