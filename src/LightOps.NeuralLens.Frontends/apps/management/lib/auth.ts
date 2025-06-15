import { IronSession, SessionOptions, getIronSession } from 'iron-session'
import { cookies } from 'next/headers'
import * as client from 'openid-client'

// Sessions are split to keep under cookie max size

// #region Internal session functions
function getSessionOptions(sessionName: string): SessionOptions {
  return {
    password: process.env.SESSION_PASSWORD!,
    cookieName: `lightops_neurallens_${sessionName}`,
    cookieOptions: {
      // secure only works in `https` environments
      // if your localhost is not on `https`, then use: `secure: process.env.NODE_ENV === "production"`
      secure: process.env.NODE_ENV === 'production',
    },
    ttl: 60 * 60 * 24 * 7, // 1 week
  }
}

async function getSession<T extends object>(sessionName: string): Promise<IronSession<T>> {
  const cookiesList = await cookies()
  return await getIronSession<T>(cookiesList, getSessionOptions(sessionName))
}

async function clearSession(sessionName: string): Promise<void> {
  const session = await getSession(sessionName)
  session.destroy()
}
// #endregion

// #region Access token refreshing
export async function refreshAccessToken(): Promise<IronSession<AccessTokenSessionData>> {
    const openIdClientConfig = await getClientConfig()
    const accessTokenSession = await AccessTokenSession.getSession()
    const refreshTokenSession = await RefreshTokenSession.getSession()

    try {
        const tokenSet = await client.refreshTokenGrant(openIdClientConfig, refreshTokenSession.refreshToken!)
        
        // Update sessions
        accessTokenSession.accessToken = tokenSet.access_token!
        accessTokenSession.save()
        refreshTokenSession.refreshToken = tokenSet.refresh_token ?? refreshTokenSession.refreshToken
        refreshTokenSession.save()

        return accessTokenSession
    } catch (error) {
        console.error('Token refresh failed:', error)
        throw new Error('Unable to refresh access token')
    }
}
// #endregion

// #region Client configuration
export const clientConfig = {
  url: process.env.NEXT_PUBLIC_AUTH_API_URL,
  audience: process.env.NEXT_PUBLIC_AUTH_API_URL,
  clientId: process.env.NEXT_PUBLIC_AUTH_CLIENT_ID,
  scope: 'openid profile offline_access'
    + ' organizations:read organizations:write'
    + ' workspaces:read workspaces:write',
  redirectUri: `${process.env.NEXT_PUBLIC_APP_URL}/api/auth/sign-in-oidc-callback`,
  response_type: 'code',
  grant_type: 'authorization_code',
  postLoginRoute: `${process.env.NEXT_PUBLIC_APP_URL}`,
  postLogoutRedirectUri: `${process.env.NEXT_PUBLIC_APP_URL}`,
  codeChallengeMethod: 'S256',
}

export async function getClientConfig() {
  return await client.discovery(new URL(clientConfig.url!), clientConfig.clientId!)
}
// #endregion

// #region Auth session
const authSessionName = 'auth'

export interface AuthSessionData {
  isLoggedIn: boolean
  userInfo: {
    id: string
    name: string
    picture: string
    updated_at: string
  } | object
}

export const AuthSession = {
  getSession: async () => {
    const session = await getSession<AuthSessionData>(authSessionName)
    if (!session.isLoggedIn) {
      session.isLoggedIn = false
      session.userInfo = {}
    }
    return session
  },
  clearSession: async () => clearSession(authSessionName),
}
// #endregion

// #region Access token session
const accessTokenSessionName = 'at'

export interface AccessTokenSessionData {
  accessToken?: string
}

export const AccessTokenSession = {
  getSession: async () => getSession<AccessTokenSessionData>(accessTokenSessionName),
  clearSession: async () => clearSession(accessTokenSessionName)
}
// #endregion

// #region Refresh token session
const refreshTokenSessionName = 'rt'

export interface RefreshTokenSessionData {
  refreshToken?: string
}

export const RefreshTokenSession = {
  getSession: async () => getSession<RefreshTokenSessionData>(refreshTokenSessionName),
  clearSession: async () => clearSession(refreshTokenSessionName)
}
// #endregion

// #region Sign-in session
const signInSessionName = 'signin'

export interface SignInSessionData {
  codeVerifier?: string
  state?: string
}

export const SignInSession = {
  getSession: async () => getSession<SignInSessionData>(signInSessionName),
  clearSession: async () => clearSession(signInSessionName)
}
// #endregion
