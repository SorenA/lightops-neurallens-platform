import { getClientConfig, clientConfig, AuthSession, AccessTokenSession, RefreshTokenSession } from '@/lib/auth'
import * as client from 'openid-client'

export async function GET() {
  const accessTokenSession = await AccessTokenSession.getSession()

  // Build authorization url
  const openIdClientConfig = await getClientConfig()
  const endSessionUrl = client.buildEndSessionUrl(openIdClientConfig, {
    post_logout_redirect_uri: clientConfig.postLogoutRedirectUri,
    id_token_hint: accessTokenSession.accessToken!,
  })
  
  // Clear sessions
  await AuthSession.clearSession()
  await AccessTokenSession.clearSession()
  await RefreshTokenSession.clearSession()
  
  // Redirect user to end session endpoint
  return Response.redirect(endSessionUrl.href)
}
