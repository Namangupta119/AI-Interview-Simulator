import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../hooks/AuthContext";
import {
  getDashboard,
  type DashboardRecentInterviewResponse,
  type DashboardResponse,
} from "../api/dashboardApi";

const statusLabels: Record<number, string> = {
  0: "In Progress",
  1: "Completed",
  2: "Abandoned",
};

function formatDate(date: string): string {
  return new Date(date).toLocaleDateString("en-IN", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
}

function getScoreClass(score: number | null): string {
  if (score === null) {
    return "dashboard-score-pending";
  }

  if (score >= 7) {
    return "dashboard-score-good";
  }

  if (score >= 5) {
    return "dashboard-score-average";
  }

  return "dashboard-score-low";
}

function RecentInterviewCard({
  interview,
}: {
  interview: DashboardRecentInterviewResponse;
}) {
  const isCompleted = interview.status === 1;
  const isAbandoned = interview.status === 2;

  return (
    <div className="dashboard-recent-item">
      <div className="dashboard-recent-info">
        <strong>{interview.role}</strong>

        <span>
          {formatDate(interview.startedAtUtc)}
        </span>
      </div>

      <div className="dashboard-recent-meta">
        <span
          className={`dashboard-recent-status ${isCompleted
            ? "dashboard-recent-status-completed"
            : isAbandoned
              ? "dashboard-recent-status-abandoned"
              : "dashboard-recent-status-progress"
            }`}
        >
          {statusLabels[interview.status] ?? "Unknown"}
        </span>

        <strong
          className={getScoreClass(interview.overallScore)}
        >
          {interview.overallScore !== null
            ? `${interview.overallScore.toFixed(1)}/10`
            : "Pending"}
        </strong>

        {interview.hasReport ? (
          <Link
            to={`/report/${interview.sessionId}`}
            className="dashboard-recent-action"
          >
            View Report →
          </Link>
        ) : isCompleted ? (
          <span className="dashboard-recent-muted">
            Report unavailable
          </span>
        ) : isAbandoned ? (
          <span className="dashboard-recent-muted">
            Interview abandoned
          </span>
        ) : (
          <Link
            to={`/interview/${interview.sessionId}`}
            className="dashboard-recent-action"
          >
            Continue →
          </Link>
        )}
      </div>
    </div>
  );
}

export function DashboardPage() {
  const { user } = useAuth();

  const [dashboard, setDashboard] =
    useState<DashboardResponse | null>(null);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadDashboard() {
      try {
        setError("");

        const response = await getDashboard();

        setDashboard(response);
      } catch {
        setError("Unable to load dashboard data.");
      } finally {
        setIsLoading(false);
      }
    }

    loadDashboard();
  }, []);

  return (
    <div className="dashboard">
      <section className="dashboard-header">
        <div>
          <p className="dashboard-eyebrow">
            AI INTERVIEW SIMULATOR
          </p>

          <h1>Welcome, {user?.fullName}</h1>

          <p className="dashboard-description">
            Prepare yourself for your next AI-powered interview.
          </p>
        </div>
      </section>

      {isLoading ? (
        <div className="dashboard-data-state">
          Loading your dashboard...
        </div>
      ) : error ? (
        <div className="dashboard-data-state dashboard-data-error">
          {error}
        </div>
      ) : dashboard ? (
        <>
          <section className="dashboard-stats">
            <div className="dashboard-stat-card">
              <span className="dashboard-stat-label">
                Total Interviews
              </span>

              <strong>{dashboard.totalInterviews}</strong>
            </div>

            <div className="dashboard-stat-card">
              <span className="dashboard-stat-label">
                Completed Interviews
              </span>

              <strong>
                {dashboard.completedInterviews}
              </strong>
            </div>

            <div className="dashboard-stat-card">
              <span className="dashboard-stat-label">
                Average Score
              </span>

              <strong>
                {dashboard.averageScore !== null
                  ? `${dashboard.averageScore.toFixed(1)}/10`
                  : "N/A"}
              </strong>
            </div>
          </section>

          <section className="dashboard-actions">
            <Link
              to="/interview/setup"
              className="dashboard-card dashboard-card-primary"
            >
              <span className="dashboard-card-icon">
                🎯
              </span>

              <div>
                <h2>Start New Interview</h2>
                <p>
                  Practice with an AI interviewer and get
                  instant feedback.
                </p>
              </div>

              <span className="dashboard-card-arrow">
                →
              </span>
            </Link>

            <Link
              to="/history"
              className="dashboard-card"
            >
              <span className="dashboard-card-icon">
                📊
              </span>

              <div>
                <h2>Interview History</h2>
                <p>
                  Review your previous interviews and
                  performance.
                </p>
              </div>

              <span className="dashboard-card-arrow">
                →
              </span>
            </Link>
          </section>

          <section className="dashboard-recent">
            <div className="dashboard-recent-header">
              <div>
                <p className="dashboard-eyebrow">
                  RECENT ACTIVITY
                </p>

                <h2>Recent Interviews</h2>
              </div>

              {dashboard.recentInterviews.length > 0 && (
                <Link to="/history">
                  View All →
                </Link>
              )}
            </div>

            {dashboard.recentInterviews.length === 0 ? (
              <div className="dashboard-data-state">
                You haven't started any interviews yet.
              </div>
            ) : (
              <div className="dashboard-recent-list">
                {dashboard.recentInterviews.map(
                  (interview) => (
                    <RecentInterviewCard
                      key={interview.sessionId}
                      interview={interview}
                    />
                  ),
                )}
              </div>
            )}
          </section>
        </>
      ) : null}
    </div>
  );
}