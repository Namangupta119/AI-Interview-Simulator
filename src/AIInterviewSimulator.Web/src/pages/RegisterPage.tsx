import { Link } from "react-router-dom";
import { RegisterForm } from "../components/RegisterForm";

export function RegisterPage() {
  return (
    <main className="auth-page">
      <div className="auth-card">
        <h1>Create account</h1>

        <RegisterForm />

        <p>
          Already have an account?{" "}
          <Link to="/login">Sign in</Link>
        </p>
      </div>
    </main>
  );
}