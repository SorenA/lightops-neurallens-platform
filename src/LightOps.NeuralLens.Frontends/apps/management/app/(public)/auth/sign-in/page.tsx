import SignIn from "@/components/sign-in"

export default function Page() {
  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-col gap-6">
        <SignIn />
      </div>
      <div className="text-muted-foreground *:[a]:hover:text-primary text-center text-xs text-balance *:[a]:underline *:[a]:underline-offset-4">
        By clicking continue, you agree to our <a href="#">Terms of Service</a>{" "}
        and <a href="#">Privacy Policy</a>.
      </div>
    </div>
  )
}
