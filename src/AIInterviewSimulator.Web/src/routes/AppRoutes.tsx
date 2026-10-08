import { Routes, Route } from "react-router-dom";
import { ProtectedRoute } from "./ProtectedRoute";
import { LoginPage } from "../pages/LoginPage";
import { RegisterPage } from "../pages/RegisterPage";
import { DashboardPage } from "../pages/DashboardPage";
import { AppLayout } from "../layouts/AppLayout";
import { InterviewSetupPage } from "../pages/interview/InterviewSetupPage";
import { ActiveInterviewPage } from "../pages/interview/ActiveInterviewPage";
import { ReportPage } from "../pages/report/ReportPage";

function HomePage() {
  return <h1>AI Interview Simulator</h1>;
}


function HistoryPage() {
  return <h1>Interview History</h1>;
}

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/interview/setup" element={<InterviewSetupPage />} />
          <Route path="/interview/:id" element={<ActiveInterviewPage />} />
          <Route path="/report/:id" element={<ReportPage />} />
          <Route path="/history" element={<HistoryPage />} />
        </Route>
      </Route>
    </Routes>
  );
}