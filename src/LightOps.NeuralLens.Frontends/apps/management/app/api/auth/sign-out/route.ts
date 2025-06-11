import { defaultSession, getClientConfig, getSession, clientConfig } from '@/lib/auth'
import * as client from 'openid-client'

export async function GET() {
  const session = await getSession()

  // Build authorization url
  const openIdClientConfig = await getClientConfig()
  const endSessionUrl = client.buildEndSessionUrl(openIdClientConfig, {
    post_logout_redirect_uri: clientConfig.post_logout_redirect_uri,
    id_token_hint: session.access_token!,
  })
  
  // Update session
  session.isLoggedIn = defaultSession.isLoggedIn
  session.access_token = defaultSession.access_token
  session.userInfo = defaultSession.userInfo
  session.code_verifier = defaultSession.code_verifier
  session.state = defaultSession.state
  await session.save()
  
  // Redirect user to end session endpoint
  return Response.redirect(endSessionUrl.href)
}
