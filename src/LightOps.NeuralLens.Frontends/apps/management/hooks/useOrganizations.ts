import { useEffect, useMemo, useState } from 'react'
import { Configuration, OrganizationApi, OrganizationViewModel } from '@repo/api-clients/organization'

export default function useOrganizations() {
  const [organizations, setOrganizations] = useState<OrganizationViewModel[]>([])
  const [loading, setLoading] = useState(true)

  const configuration = useMemo(() => new Configuration({
    basePath: '/api/proxy/organization',
  }), [])
  const apiClient = useMemo(() => new OrganizationApi(configuration), [configuration])

  useEffect(() => {
    // Fetch organizations
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
  }, [apiClient, configuration])
  return { organizations, loading }
}
