import { httpClient } from "./httpClient";

export interface InterviewReportResponse {
  reportId: string;
  sessionId: string;
  overallScore: number;
  strengthSummary: string;
  weaknessSummary: string;
  recommendedTopics: string;
  improvementPlan: string;
  topicScoresJson: string;
  createdAtUtc: string;
}

export async function getInterviewReport(
  sessionId: string,
): Promise<InterviewReportResponse> {
  const response =
    await httpClient.get<InterviewReportResponse>(
      `/api/interviews/sessions/${sessionId}/report`,
    );

  return response.data;
}