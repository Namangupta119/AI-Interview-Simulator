import { Link } from "react-router-dom";
import { LoginForm } from "../components/LoginForm";

export function LoginPage() {
  return (
    <main className="auth-page">
      <div className="auth-card">
        <h1>Sign in</h1>

        <LoginForm />

        <p>
          Don't have an account?{" "}
          <Link to="/register">Create an account</Link>
        </p>
      </div>
    </main>
  );
}