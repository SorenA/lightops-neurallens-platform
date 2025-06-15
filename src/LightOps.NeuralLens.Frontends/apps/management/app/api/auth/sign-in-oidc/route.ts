import { getClientConfig, clientConfig, SignInSession } from '@/lib/auth'
import * as client from 'openid-client'

export async function GET() {

  // Build authorization url
  const codeVerifier = client.randomPKCECodeVerifier()
  const codeChallenge = await client.calculatePKCECodeChallenge(codeVerifier)
  const openIdClientConfig = await getClientConfig()
  const parameters: Record<string, string> = {
    redirect_uri: clientConfig.redirectUri,
    scope: clientConfig.scope,
    code_challenge: codeChallenge,
    code_challenge_method: clientConfig.codeChallengeMethod,
  }

  let state!: string
  if (!openIdClientConfig.serverMetadata().supportsPKCE()) {
    state = client.randomState()
    parameters.state = state
  }

  const redirectTo = client.buildAuthorizationUrl(openIdClientConfig, parameters)

  // Update session
  const signInSession = await SignInSession.getSession()
  signInSession.codeVerifier = codeVerifier
  signInSession.state = state
  await signInSession.save()

  // Redirect user to sign in endpoint
  return Response.redirect(redirectTo.href)
}
