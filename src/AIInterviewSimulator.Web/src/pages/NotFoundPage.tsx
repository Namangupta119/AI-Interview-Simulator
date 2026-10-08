import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <div className="not-found-page">
      <div className="not-found-content">
        <p className="not-found-code">404</p>

        <h1>Page Not Found</h1>

        <p className="not-found-message">
          The page you’re looking for doesn’t exist or may no longer be
          available.
        </p>

        <div className="not-found-actions">
          <Link to="/dashboard" className="not-found-primary">
            Go to Dashboard
          </Link>

          <Link to="/history" className="not-found-secondary">
            View Interview History
          </Link>
        </div>
      </div>
    </div>
  );
}