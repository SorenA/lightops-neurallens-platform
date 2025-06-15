import { AuthSession } from '@/lib/auth';

export async function GET() {
  try {
    const authSession = await AuthSession.getSession()
    if (!authSession) {
      return Response.json({

      })
    }
    return Response.json({
      isLoggedIn: authSession.isLoggedIn,
      userInfo: authSession.userInfo,
    })
  } catch (e) {
    return Response.json({ error: e }, { status: 500 })
  }
}
