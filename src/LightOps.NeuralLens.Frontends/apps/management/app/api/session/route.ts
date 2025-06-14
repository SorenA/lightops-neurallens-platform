import { defaultSession, getSession } from '@/lib/auth';

export async function GET() {
  try {
    const session = await getSession()
    if (!session) {
      return Response.json({ defaultSession })
    }
    return Response.json({
      access_token: session.access_token, // Passed to allow calling APIs from frontend, should not be exposed if all calls are made through local APIs
      isLoggedIn: session.isLoggedIn,
      userInfo: session.userInfo,
    })
  } catch (e) {
    return Response.json({ error: e }, { status: 500 })
  }
}
