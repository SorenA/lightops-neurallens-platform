import { getClientConfig, clientConfig, SignInSession, AuthSession, AccessTokenSession, RefreshTokenSession } from '@/lib/auth'
import { headers } from 'next/headers'
import { NextRequest } from 'next/server'
import * as client from 'openid-client'

export async function GET(request: NextRequest) {
  const signInSession = await SignInSession.getSession()

  // Validate the authentication response
  const openIdClientConfig = await getClientConfig()
  const headerList = await headers()
  const host = headerList.get('x-forwarded-host') || headerList.get('host') || 'localhost'
  const protocol = headerList.get('x-forwarded-proto') || 'https'
  const currentUrl = new URL(
    `${protocol}://${host}${request.nextUrl.pathname}${request.nextUrl.search}`
  )
  const tokenSet = await client.authorizationCodeGrant(openIdClientConfig, currentUrl, {
    pkceCodeVerifier: signInSession.codeVerifier,
    expectedState: signInSession.state,
  })

  // Extract access token and claims
  const { access_token, refresh_token } = tokenSet
  const claims = tokenSet.claims()!
  const { sub } = claims
  
  // Call userinfo endpoint to get user info
  const userinfo = await client.fetchUserInfo(openIdClientConfig, access_token, sub)
  
  // Clear sign-in session
  await SignInSession.clearSession()

  // Update auth sessions
  const authSession = await AuthSession.getSession()
  authSession.isLoggedIn = true
  authSession.userInfo = {
    id: userinfo.sub,
    name: userinfo.name!,
    picture: userinfo.picture!,
    updated_at: (userinfo.updated_at! as any),
  }
  await authSession.save()

  // Update access token session
  const accessTokenSession = await AccessTokenSession.getSession()
  accessTokenSession.accessToken = access_token
  await accessTokenSession.save()

  // Update refresh token session
  const refreshTokenSession = await RefreshTokenSession.getSession()
  refreshTokenSession.refreshToken = refresh_token
  await refreshTokenSession.save()

  // Redirect user to post login route
  return Response.redirect(clientConfig.postLoginRoute)
}
