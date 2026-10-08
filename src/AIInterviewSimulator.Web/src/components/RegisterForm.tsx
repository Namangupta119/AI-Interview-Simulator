import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import {
  register as registerApi,
  type RegisterRequest,
} from "../api/authApi";

export function RegisterForm() {
  const navigate = useNavigate();

  const [formData, setFormData] = useState<RegisterRequest>({
    email: "",
    username: "",
    fullName: "",
    password: "",
  });

  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  function handleChange(
    event: React.ChangeEvent<HTMLInputElement>,
  ) {
    const { name, value } = event.target;

    setFormData((current) => ({
      ...current,
      [name]: value,
    }));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setError("");

    if (
      !formData.email.trim() ||
      !formData.username.trim() ||
      !formData.fullName.trim() ||
      !formData.password
    ) {
      setError("All fields are required.");
      return;
    }

    try {
      setIsSubmitting(true);

      await registerApi({
        email: formData.email.trim(),
        username: formData.username.trim(),
        fullName: formData.fullName.trim(),
        password: formData.password,
      });

      navigate("/login", { replace: true });
    } catch {
      setError("Registration failed. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label htmlFor="fullName">Full name</label>
        <input
          id="fullName"
          name="fullName"
          type="text"
          value={formData.fullName}
          onChange={handleChange}
          autoComplete="name"
          disabled={isSubmitting}
        />
      </div>

      <div>
        <label htmlFor="username">Username</label>
        <input
          id="username"
          name="username"
          type="text"
          value={formData.username}
          onChange={handleChange}
          autoComplete="username"
          disabled={isSubmitting}
        />
      </div>

      <div>
        <label htmlFor="register-email">Email</label>
        <input
          id="register-email"
          name="email"
          type="email"
          value={formData.email}
          onChange={handleChange}
          autoComplete="email"
          disabled={isSubmitting}
        />
      </div>

      <div>
        <label htmlFor="register-password">Password</label>
        <input
          id="register-password"
          name="password"
          type="password"
          value={formData.password}
          onChange={handleChange}
          autoComplete="new-password"
          disabled={isSubmitting}
        />
      </div>

      {error && <p role="alert">{error}</p>}

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Creating account..." : "Create account"}
      </button>
    </form>
  );
}