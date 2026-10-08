import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/AuthContext";

export function AppLayout() {
  const { user, logout } = useAuth();

  return (
    <div className="app-layout">
      <aside className="app-sidebar">
        <div className="app-brand">
          <h2>AI Interview</h2>
          <span>Simulator</span>
        </div>

        <nav className="app-nav">
          <NavLink to="/dashboard">Dashboard</NavLink>
          <NavLink to="/interview/setup">New Interview</NavLink>
          <NavLink to="/history">History</NavLink>
        </nav>

        <div className="app-sidebar-footer">
          <span>{user?.username}</span>

          <button type="button" onClick={logout}>
            Logout
          </button>
        </div>
      </aside>

      <main className="app-main">
        <Outlet />
      </main>
    </div>
  );
}