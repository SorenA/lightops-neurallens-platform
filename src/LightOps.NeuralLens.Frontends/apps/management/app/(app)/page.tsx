import { AuthSession } from "@/lib/auth"
import { Button } from "@repo/ui/components/button"

export default async function Page() {
  const authSession = await AuthSession.getSession()

  return (
    <div className="flex items-center justify-center min-h-svh">
      <div className="flex flex-col items-center justify-center gap-4">
        <h1 className="text-2xl font-bold">Hello World</h1>
        <Button size="sm">Button</Button>
        <pre>{JSON.stringify(authSession, null, 2)}</pre>
      </div>
      
    </div>
  )
}
