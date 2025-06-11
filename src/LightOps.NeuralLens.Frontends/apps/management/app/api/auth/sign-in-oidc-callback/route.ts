import { getClientConfig, getSession, clientConfig } from '@/lib/auth'
import { headers } from 'next/headers'
import { NextRequest } from 'next/server'
import * as client from 'openid-client'

export async function GET(request: NextRequest) {
  const session = await getSession()

  // Validate the authentication response
  const openIdClientConfig = await getClientConfig()
  const headerList = await headers()
  const host = headerList.get('x-forwarded-host') || headerList.get('host') || 'localhost'
  const protocol = headerList.get('x-forwarded-proto') || 'https'
  const currentUrl = new URL(
    `${protocol}://${host}${request.nextUrl.pathname}${request.nextUrl.search}`
  )
  const tokenSet = await client.authorizationCodeGrant(openIdClientConfig, currentUrl, {
    pkceCodeVerifier: session.code_verifier,
    expectedState: session.state,
  })

  // Extract access token and claims
  const { access_token } = tokenSet
  const claims = tokenSet.claims()!
  const { sub } = claims
  
  // Call userinfo endpoint to get user info
  const userinfo = await client.fetchUserInfo(openIdClientConfig, access_token, sub)
  
  // Update session
  session.isLoggedIn = true
  session.access_token = access_token
  session.userInfo = {
    id: userinfo.sub,
    name: userinfo.name!,
    picture: userinfo.picture!,
    updated_at: (userinfo.updated_at! as any),
  }
  await session.save()

  // Redirect user to post login route
  return Response.redirect(clientConfig.post_login_route)
}
