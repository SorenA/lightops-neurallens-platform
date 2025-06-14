import { useEffect, useMemo, useState } from 'react'
import { Configuration, OrganizationApi, OrganizationViewModel } from '@repo/api-clients/organization'
import useSession from './useSession'

export default function useOrganizations() {
  const [organizations, setOrganizations] = useState<OrganizationViewModel[]>([])
  const [loading, setLoading] = useState(true)

  const { session } = useSession()
  const configuration = useMemo(() => new Configuration({
    basePath: process.env.NEXT_PUBLIC_ORGANIZATION_API_URL,
    accessToken: session?.access_token,
  }), [session]);
  const apiClient = useMemo(() => new OrganizationApi(configuration), [configuration]);

  useEffect(() => {
    const fetchOrganizations = async () => {
      try {
        const response = await apiClient.getOrganizations()
        if (response) {
          setOrganizations(response)
        }
      } catch {
        setOrganizations([])
      } finally {
        setLoading(false)
      }
    }
    fetchOrganizations()
  }, [apiClient])
  return { organizations, loading }
}
