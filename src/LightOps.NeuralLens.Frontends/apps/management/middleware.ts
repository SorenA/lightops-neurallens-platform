import { NextRequest, NextResponse } from 'next/server'
import { getSession } from '@/lib/auth'

// Specify protected and public route prefixes
const protectedRoutes = ['/']
const protectedRoutePrefixes = ['/dashboard']
const publicRoutePrefixes = ['/auth/']

export default async function middleware(req: NextRequest) {
  // Check if the current route is protected or public
  const path = req.nextUrl.pathname
  const isProtectedRoute = protectedRoutes.includes(path)
    || protectedRoutePrefixes.some((p) => { return path.startsWith(p) })
  const isPublicRoute = publicRoutePrefixes.some((p) => { return path.startsWith(p) })

  // Get session
  const session = await getSession()

  // Redirect to sign in if the user is not authenticated
  if (isProtectedRoute && !session.isLoggedIn) {
    return NextResponse.redirect(new URL('/auth/sign-in', req.nextUrl))
  }

  // Redirect to default page if the user is authenticated
  if (
    isPublicRoute &&
    session.isLoggedIn &&
    req.nextUrl.pathname != '/'
  ) {
    return NextResponse.redirect(new URL('/', req.nextUrl))
  }

  return NextResponse.next()
}

// Routes Middleware should not run on
export const config = {
  matcher: ['/((?!api|_next/static|_next/image|.*\\.png$).*)'],
}