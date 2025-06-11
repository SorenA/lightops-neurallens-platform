import { BrainCircuit } from "lucide-react"

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <div className="bg-background flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
      <div className="flex flex-col gap-6">
        <div className="flex flex-col gap-6">
          <div className="flex flex-col items-center gap-2">
            <a
              href="#"
              className="flex flex-col items-center gap-2 font-medium"
            >
              <div className="flex size-8 items-center justify-center rounded-md">
                <BrainCircuit className="size-6" />
              </div>
              <span className="sr-only">LightOps NeuralLink</span>
            </a>
            <h1 className="text-xl font-bold">Welcome to LightOps NeuralLink</h1>
            <div className="text-center text-sm">
              Don&apos;t have an account?{" "}
              <a href="#" className="underline underline-offset-4">
                Get in touch
              </a>
            </div>
          </div>
          {children}
        </div>
      </div>
    </div>
  )
}
