import { Link } from "react-router-dom";
import { useAuth } from "../hooks/AuthContext";

export function DashboardPage() {
  const { user } = useAuth();

  return (
    <div className="dashboard">
      <section className="dashboard-header">
        <div>
          <p className="dashboard-eyebrow">AI INTERVIEW SIMULATOR</p>

          <h1>Welcome, {user?.fullName}</h1>

          <p className="dashboard-description">
            Prepare yourself for your next AI-powered interview.
          </p>
        </div>
      </section>

      <section className="dashboard-actions">
        <Link to="/interview/setup" className="dashboard-card dashboard-card-primary">
          <span className="dashboard-card-icon">🎯</span>

          <div>
            <h2>Start New Interview</h2>
            <p>
              Practice with an AI interviewer and get instant feedback.
            </p>
          </div>

          <span className="dashboard-card-arrow">→</span>
        </Link>

        <Link to="/history" className="dashboard-card">
          <span className="dashboard-card-icon">📊</span>

          <div>
            <h2>Interview History</h2>
            <p>
              Review your previous interviews and performance.
            </p>
          </div>

          <span className="dashboard-card-arrow">→</span>
        </Link>
      </section>
    </div>
  );
}