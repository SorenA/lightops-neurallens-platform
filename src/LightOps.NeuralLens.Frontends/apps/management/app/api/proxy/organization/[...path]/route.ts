import { AuthSession, AccessTokenSession, RefreshTokenSession, refreshAccessToken } from '@/lib/auth'
import { NextRequest, NextResponse } from 'next/server'

const apiUrl = process.env.NEXT_PUBLIC_ORGANIZATION_API_URL
const localApiUrlPrefix = '/api/proxy/organization'

async function handleRequest(request: NextRequest) {
    // Get current sessions
    const authSession = await AuthSession.getSession()
    let accessTokenSession = await AccessTokenSession.getSession()
    const refreshTokenSession = await RefreshTokenSession.getSession()

    // Check if user is logged in
    if (!authSession.isLoggedIn || !accessTokenSession.accessToken) {
        return NextResponse.json({ error: 'Unauthorized' }, { status: 401 })
    }

    // Check if we have a refresh token
    if (!refreshTokenSession.refreshToken) {
        return NextResponse.json({ error: 'No refresh token available' }, { status: 401 })
    }

    let accessToken = accessTokenSession.accessToken

    // Refresh token if not available
    if (!accessToken) {
        try {
            accessTokenSession = await refreshAccessToken()
            accessToken = accessTokenSession.accessToken!
        } catch (error) {
            return NextResponse.json({ error: 'Token refresh failed' }, { status: 401 })
        }
    }

    // Proxy the request to the API
    const proxyPath = request.nextUrl.pathname.substring(localApiUrlPrefix.length)
    try {
        if (!apiUrl) {
            throw new Error('API URL not configured')
        }

        // Clone the original request to avoid consuming its body
        const requestBody = await request.text()
        const headers = new Headers(request.headers)
        
        // Add Authorization header
        headers.set('Authorization', `Bearer ${accessToken}`)
        headers.set('Content-Type', 'application/json')

        const proxyResponse = await fetch(`${apiUrl}${proxyPath}${request.nextUrl.search}`, {
            method: request.method,
            headers: headers,
            body: requestBody || undefined,
        })

        // If unauthorized, attempt to refresh token and retry
        if (proxyResponse.status === 401) {
            try {
                const refreshedTokens = await refreshAccessToken()
                accessToken = refreshedTokens.accessToken!
                
                // Retry the request with the new token
                headers.set('Authorization', `Bearer ${accessToken}`)
                const retryResponse = await fetch(`${apiUrl}${proxyPath}${request.nextUrl.search}`, {
                    method: request.method,
                    headers: headers,
                    body: requestBody || undefined,
                })

                return new NextResponse(retryResponse.body, {
                    status: retryResponse.status,
                    headers: retryResponse.headers,
                })
            } catch (refreshError) {
                return NextResponse.json({ error: 'Unable to refresh token' }, { status: 401 })
            }
        }

        // Return the proxy response
        return new NextResponse(proxyResponse.body, {
            status: proxyResponse.status,
            headers: proxyResponse.headers,
        })
    } catch (error) {
        console.error('API proxy error:', error)
        return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 })
    }
}

export async function GET(request: NextRequest) {
    return handleRequest(request)
}

export async function HEAD(request: NextRequest) {
    return handleRequest(request)
}

export async function POST(request: NextRequest) {
    return handleRequest(request)
}

export async function PUT(request: NextRequest) {
    return handleRequest(request)
}

export async function DELETE(request: NextRequest) {
    return handleRequest(request)
}

export async function PATCH(request: NextRequest) {
    return handleRequest(request)
}
