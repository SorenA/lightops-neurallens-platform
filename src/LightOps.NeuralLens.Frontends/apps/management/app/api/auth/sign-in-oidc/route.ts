import { getClientConfig, getSession, clientConfig } from '@/lib/auth'
import * as client from 'openid-client'

export async function GET() {
  const session = await getSession()

  // Build authorization url
  const codeVerifier = client.randomPKCECodeVerifier()
  const codeChallenge = await client.calculatePKCECodeChallenge(codeVerifier)
  const openIdClientConfig = await getClientConfig()
  const parameters: Record<string, string> = {
    redirect_uri: clientConfig.redirect_uri,
    scope: clientConfig.scope,
    code_challenge: codeChallenge,
    code_challenge_method: clientConfig.code_challenge_method,
  }

  let state!: string
  if (!openIdClientConfig.serverMetadata().supportsPKCE()) {
    state = client.randomState()
    parameters.state = state
  }

  const redirectTo = client.buildAuthorizationUrl(openIdClientConfig, parameters)

  // Update session
  session.code_verifier = codeVerifier
  session.state = state
  await session.save()

  // Redirect user to sign in endpoint
  return Response.redirect(redirectTo.href)
}
