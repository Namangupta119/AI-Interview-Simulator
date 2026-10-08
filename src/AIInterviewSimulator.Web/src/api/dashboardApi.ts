import { httpClient } from "./httpClient";

export interface DashboardRecentInterviewResponse {
  sessionId: string;
  role: string;
  overallScore: number | null;
  status: number;
  startedAtUtc: string;
  hasReport: boolean;
}

export interface DashboardResponse {
  totalInterviews: number;
  completedInterviews: number;
  averageScore: number | null;
  recentInterviews: DashboardRecentInterviewResponse[];
}

export async function getDashboard(): Promise<DashboardResponse> {
  const response =
    await httpClient.get<DashboardResponse>("/api/dashboard");

  return response.data;
}